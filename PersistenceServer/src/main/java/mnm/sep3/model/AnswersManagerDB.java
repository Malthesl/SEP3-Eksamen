package mnm.sep3.model;

import mnm.sep3.database.Database;
import mnm.sep3.model.entities.Answer;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class AnswersManagerDB implements AnswersManager {
    private final Connection connection = Database.getConnection();

    private final Map<Integer, Answer> answers = new HashMap<>();

    @Override
    public Answer getAnswer(int id) {
        Answer returnAnswer = answers.get(id);
        if (returnAnswer != null) return returnAnswer;

        try {
            PreparedStatement statement = connection.prepareStatement("SELECT * FROM answers WHERE id = ?");
            statement.setInt(1, id);

            ResultSet res = statement.executeQuery();
            if (res.next()) {
                int questionId = res.getInt("question_id");
                String title = res.getString("title");
                boolean correct = res.getBoolean("is_correct");
                int index = res.getInt("index");

                returnAnswer = new Answer(questionId, id, title, correct, index);
                answers.put(id, returnAnswer);
                return returnAnswer;
            }
            throw new RuntimeException("Kunne ikke finde et svar med ID: " + id);
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public int addAnswer(int questionId, String title, boolean isCorrect, int index) {
        if (title.isBlank()) throw new IllegalArgumentException("Titel må ikke være blank");

        try {
            PreparedStatement statement = connection.prepareStatement("INSERT INTO answers (title, index, is_correct, question_id) VALUES (?, ?, ?, ?) RETURNING id");
            statement.setString(1, title);
            statement.setInt(2, index);
            statement.setBoolean(3, isCorrect);
            statement.setInt(4, questionId);

            ResultSet res = statement.executeQuery();
            if (res.next()) {
                int id = res.getInt("id");
                Answer returnAnswer = new Answer(questionId, id, title, isCorrect, index);
                answers.put(id, returnAnswer);
                return id;
            }
            throw new RuntimeException("Kunne ikke indsætte");
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public void updateAnswer(Answer answer) {
        if (answer.getTitle().isBlank()) throw new IllegalArgumentException("Titel må ikke være blank");

        try {
            PreparedStatement statement = connection.prepareStatement("UPDATE answers SET title = ?, is_correct = ? WHERE id = ? AND question_id = ?");
            statement.setString(1, answer.getTitle());
            statement.setBoolean(2, answer.isCorrect());
            statement.setInt(3, answer.getAnswerId());
            statement.setInt(4, answer.getQuestionId());
            statement.execute();

            answers.put(answer.getAnswerId(), answer);
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public void deleteAnswer(int answerId) {
        try {
            PreparedStatement statement = connection.prepareStatement("DELETE FROM answers WHERE id = ?");
            statement.setInt(1, answerId);
            statement.execute();

            answers.remove(answerId);
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public List<Answer> getAllAnswersForQuestion(int questionId) {
        try {
            PreparedStatement statement = connection.prepareStatement("SELECT * FROM answers WHERE question_id = ? ORDER BY index");
            statement.setInt(1, questionId);

            ResultSet res = statement.executeQuery();
            List<Answer> returnList = new ArrayList<>();
            while (res.next()) {
                Answer returnAnswer = new Answer(
                        questionId,
                        res.getInt("id"),
                        res.getString("title"),
                        res.getBoolean("is_correct"),
                        res.getInt("index")
                );
                returnList.add(returnAnswer);
                answers.put(returnAnswer.getAnswerId(), returnAnswer);
            }
            return returnList;
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }
}
