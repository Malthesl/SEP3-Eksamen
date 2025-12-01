package mnm.sep3.model;

import mnm.sep3.database.Database;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class QuestionsManagerDB implements QuestionsManager
{
  private final Connection connection = Database.getConnection();

  private final Map<Integer, Question> questions = new HashMap<>();

  @Override
  public int addQuestion(int quizId, String title)
  {
    try
    {
      PreparedStatement statement = connection.prepareStatement(
          "INSERT INTO questions (in_quiz_id, title, index) VALUES (?, ?, ?) RETURNING questions.id as id");
      statement.setInt(1, quizId);
      statement.setString(2, title);
      int index = getAllQuestionsInQuiz(quizId).size();
      statement.setInt(3, index);

      ResultSet res = statement.executeQuery();
      if (res.next())
      {
        int id = res.getInt("id");
        questions.put(id, new Question(id, quizId, title, index));
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

  @Override
  public void updateQuestion(Question question) {

  }

  @Override
  public void moveQuestion(int questionId, int toIndex) {

  }

  @Override
  public void deleteQuestion(int questionId) {

  }

  @Override public List<Question> getAllQuestionsInQuiz(int quizId)
  {
    List<Question> returnList = new ArrayList<>();
    try
    {
      PreparedStatement statement = connection.prepareStatement("SELECT * FROM questions WHERE in_quiz_id = ?");
      statement.setInt(1, quizId);

      ResultSet res = statement.executeQuery();

      while (res.next())
      {
        int id = res.getInt("id");
        String title = res.getString("title");
        int index = res.getInt("index");

        Question question = new Question(id, quizId, title, index);
        questions.put(id, question);
        returnList.add(question);
      }
      return returnList;
    }
    catch (SQLException e)
    {
      throw new RuntimeException(e);
    }
  }

  @Override public Question getQuestionById(int id)
  {
    Question question = questions.get(id);
    if (question != null)
      return question;

    try {
      PreparedStatement statement = connection.prepareStatement("SELECT * FROM questions WHERE id=?");
      statement.setInt(1, id);

      ResultSet res = statement.executeQuery();

      if (res.next()) {
        String title = res.getString("question");
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
