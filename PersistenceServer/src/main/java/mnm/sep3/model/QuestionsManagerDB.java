package mnm.sep3.model;

import mnm.sep3.database.Database;
import mnm.sep3.model.entities.Question;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class QuestionsManagerDB implements QuestionsManager {
    private final Connection connection = Database.getConnection();

    private final Map<Integer, Question> questions = new HashMap<>();

    @Override
    public int addQuestion(int quizId, String title) {
        try {
            PreparedStatement statement = connection.prepareStatement(
                    "INSERT INTO questions (in_quiz_id, title, index) VALUES (?, ?, ?) RETURNING questions.id as id");
            statement.setInt(1, quizId);
            statement.setString(2, title);
            // Finder det højeste ID og plusser 1. Hvis der ikke er noget, så fjern -1 (gør det til 0)
            int index = getAllQuestionsInQuiz(quizId).stream().mapToInt(Question::getIndex).max().orElse(-1) + 1;
            statement.setInt(3, index);

            ResultSet res = statement.executeQuery();
            if (res.next()) {
                int id = res.getInt("id");
                questions.put(id, new Question(id, quizId, title, index));
                return id;

            } else {
                throw new RuntimeException("Kunne ikke tilføjes");
            }
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public void updateQuestion(Question question) {
        try {
            PreparedStatement statement = connection.prepareStatement("UPDATE questions SET title=? WHERE id=? AND in_quiz_id = ?");
            statement.setString(1, question.getTitle());
            statement.setInt(2, question.getId());
            statement.setInt(3, question.getQuizId());

            statement.execute();

            questions.put(question.getId(), question);
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public void moveQuestion(int questionId, int toIndex) {
        // BBS Rokerings Algoritme Tech 2025 2.0
        try {
            Question question = getQuestionById(questionId);
            int startIndex = question.getIndex();

            // Ik gør noget hvis indekset er det samme
            if (startIndex == toIndex) return;

            PreparedStatement statement;
            if (startIndex < toIndex) {
                statement = connection.prepareStatement("UPDATE questions SET index = index - 1 WHERE in_quiz_id = ? AND index > ? AND index <= ?");
                statement.setInt(1, question.getQuizId());
                statement.setInt(2, startIndex);
                statement.setInt(3, toIndex);
            } else {
                statement = connection.prepareStatement("UPDATE questions SET index = index + 1 WHERE in_quiz_id = ? AND index >= ? AND index < ?");
                statement.setInt(1, question.getQuizId());
                statement.setInt(2, toIndex);
                statement.setInt(3, startIndex);
            }
            statement.execute();

            statement = connection.prepareStatement("UPDATE questions SET index = ? WHERE id = ?");
            statement.setInt(1, toIndex);
            statement.setInt(2, questionId);
            statement.execute();
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public void deleteQuestion(int questionId) {
        try {
            PreparedStatement statement = connection.prepareStatement("DELETE FROM questions WHERE id = ? RETURNING index");
            statement.setInt(1, questionId);
            var res = statement.executeQuery();

            if (res.next()) {
                statement = connection.prepareStatement("UPDATE questions SET index = index - 1 WHERE index > ?");
                statement.setInt(1, res.getInt("index"));
                statement.execute();
            }
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public List<Question> getAllQuestionsInQuiz(int quizId) {
        System.out.println("Der var sq nogen som leder efter spørgsmål med ID: " + quizId);
        List<Question> returnList = new ArrayList<>();
        try {
            PreparedStatement statement = connection.prepareStatement("SELECT * FROM questions WHERE in_quiz_id = ? ORDER BY index");
            statement.setInt(1, quizId);

            ResultSet res = statement.executeQuery();

            while (res.next()) {
                int id = res.getInt("id");
                String title = res.getString("title");
                int index = res.getInt("index");

                Question question = new Question(id, quizId, title, index);
                questions.put(id, question);
                returnList.add(question);
            }
            return returnList;
        } catch (SQLException e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public Question getQuestionById(int id) {
        Question question = questions.get(id);
        if (question != null)
            return question;

        try {
            PreparedStatement statement = connection.prepareStatement("SELECT * FROM questions WHERE id=?");
            statement.setInt(1, id);

            ResultSet res = statement.executeQuery();

            if (res.next()) {
                String title = res.getString("title");
                int inQuizId = res.getInt("in_quiz_id");
                int index = res.getInt("index");

                question = new Question(id, inQuizId, title, index);
                questions.put(id, question);
                return question;
            } else {
                throw new RuntimeException("Question does not exist");
            }
        } catch (SQLException e) {
            throw new RuntimeException(e);
        }
    }
}
