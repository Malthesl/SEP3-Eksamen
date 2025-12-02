package mnm.sep3.model;

import mnm.sep3.database.Database;
import mnm.sep3.model.entities.Quiz;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class QuizzesManagerDB implements QuizzesManager
{
  private final Connection connection = Database.getConnection();

  private final Map<Integer, Quiz> quizzes = new HashMap<>();

  @Override public Quiz getQuiz(int quizId)
  {
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

  @Override public int addQuiz(String title, int creatorId)
  {
    try
    {
      PreparedStatement statement = connection.prepareStatement(
          "INSERT INTO quizzes (title, creator_id, visibility) VALUES (?,?,?) RETURNING quizzes.id as id;");
      statement.setString(1, title);
      statement.setInt(2, creatorId);
      statement.setString(3, "private");

      ResultSet res = statement.executeQuery();
      if (res.next())
      {
        int id = res.getInt("id");
        quizzes.put(id, new Quiz(id, title, "private", creatorId));
        return id;
      }
      else
      {
        throw new RuntimeException("Kunne ikke tilføjes");
      }
    }
    catch (Exception e)
    {
      throw new RuntimeException(e);
    }
  }

  @Override public void updateQuiz(Quiz quiz)
  {
    try
    {
      PreparedStatement statement = connection.prepareStatement(
          "UPDATE quizzes SET title=? WHERE id=?");
      statement.setString(1, quiz.getTitle());
      statement.setInt(2, quiz.getQuizId());

      statement.execute();

      quizzes.put(quiz.getQuizId(), quiz);
    }
    catch (Exception e)
    {
      throw new RuntimeException(e);
    }

  }

  @Override public void deleteQuiz(int quizId)
  {
    try
    {
      PreparedStatement statement = connection.prepareStatement(
          "DELETE FROM quizzes WHERE id = ?");
      statement.setInt(1, quizId);
      statement.execute();
    }
    catch (Exception e)
    {
      throw new RuntimeException(e);
    }
  }

  @Override public List<Quiz> queryQuizzes(String title, int byCreator)
  {
    return List.of();
  }
}
