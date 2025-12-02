package mnm.sep3.model;

import mnm.sep3.database.Database;
import mnm.sep3.model.entities.QueryResult;
import mnm.sep3.model.entities.Quiz;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class QuizzesManagerDB implements QuizzesManager {
    private final Connection connection = Database.getConnection();

    private final Map<Integer, Quiz> quizzes = new HashMap<>();

    @Override
    public Quiz getQuiz(int quizId) {
        Quiz quiz = quizzes.get(quizId);
        if (quiz != null)
            return quiz;

        try {
            PreparedStatement statement = connection.prepareStatement("SELECT * FROM quizzes WHERE id=?");
            statement.setInt(1, quizId);

            ResultSet res = statement.executeQuery();

            if (res.next()) {
                String title = res.getString("title");
                String visibility = res.getString("visibility");
                int creatorId = res.getInt("creator_id");

                quiz = new Quiz(quizId, title, visibility, creatorId);
                quizzes.put(quizId, quiz);
                return quiz;
            } else {
                throw new RuntimeException("Quiz does not exist");
            }
        } catch (SQLException e) {
            throw new RuntimeException(e);
        }

    }

    @Override
    public int addQuiz(String title, int creatorId) {
        try {
            PreparedStatement statement = connection.prepareStatement(
                    "INSERT INTO quizzes (title, creator_id, visibility) VALUES (?,?,?) RETURNING quizzes.id as id;");
            statement.setString(1, title);
            statement.setInt(2, creatorId);
            statement.setString(3, "private");

            ResultSet res = statement.executeQuery();
            if (res.next()) {
                int id = res.getInt("id");
                quizzes.put(id, new Quiz(id, title, "private", creatorId));
                return id;
            } else {
                throw new RuntimeException("Kunne ikke tilføjes");
            }
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public void updateQuiz(Quiz quiz) {
        try {
            PreparedStatement statement = connection.prepareStatement(
                    "UPDATE quizzes SET title=? WHERE id=?");
            statement.setString(1, quiz.getTitle());
            statement.setInt(2, quiz.getQuizId());

            statement.execute();

            quizzes.put(quiz.getQuizId(), quiz);
        } catch (Exception e) {
            throw new RuntimeException(e);
        }

    }

    @Override
    public void deleteQuiz(int quizId) {
        try {
            PreparedStatement statement = connection.prepareStatement("DELETE FROM quizzes WHERE id = ?");
            statement.setInt(1, quizId);
            statement.execute();
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public QueryResult<Quiz> queryQuizzes(String searchQuery, int byCreator, int start, int end, List<String> visibilities) {
        try {
            ResultSet res;
            List<String> queryOptions = new ArrayList<>();
            List<Object> queryValues = new ArrayList<>();

            if (!searchQuery.isBlank()) {
                searchQuery = "%" + searchQuery + "%";
                queryOptions.add("title ILIKE ?");
                queryValues.add(searchQuery);
            }

            if (byCreator != -1) {
                queryOptions.add("creator_id = ?");
                queryValues.add(byCreator);
            }

            visibilities = new ArrayList<>(visibilities);

            if (visibilities.isEmpty()) visibilities.add("public");

            if (visibilities.contains("private") && byCreator == -1)
                throw new RuntimeException("Fejl i queryQuizzes: Når private bruges i visibilities, skal en byCreator inkluderes");

            queryOptions.add("visibility IN (?)");
            queryValues.add(String.join(", ", visibilities));

            PreparedStatement statement = connection.prepareStatement("SELECT COUNT(*) AS count FROM quizzes WHERE " + String.join(" AND ", queryOptions));

            int l = queryValues.size();
            
            for (int i = 1; i < l + 1; i++) {
                statement.setObject(i, queryValues.get(i - 1));
            }

            res = statement.executeQuery();

            int count = 0;

            if (res.next()) {
                count = res.getInt("count");
            }

            // Siden der aldrig bliver indsat noget direkte fra klienten ind i queryen, burde det være save
            statement = connection.prepareStatement("SELECT * FROM quizzes WHERE " + String.join(" AND ", queryOptions) + " OFFSET ? LIMIT ?");

            for (int i = 1; i < l + 1; i++) {
                statement.setObject(i, queryValues.get(i - 1));
            }

            statement.setInt(l + 1, start);
            statement.setInt(l + 2, end);

            res = statement.executeQuery();

            List<Quiz> quizzes = new ArrayList<>();

            while (res.next()) {
                int id = res.getInt("id");
                String title = res.getString("title");
                String visibility = res.getString("visibility");
                int creatorId = res.getInt("creator_id");
                quizzes.add(new Quiz(id, title, visibility, creatorId));
            }

            return new QueryResult<>(quizzes, count);
        } catch (SQLException e) {
            throw new RuntimeException(e);
        }
    }
}
