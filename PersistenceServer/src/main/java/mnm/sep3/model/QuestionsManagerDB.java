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

  @Override public int addQuestion(String question, String answer)
  {
    try
    {
      PreparedStatement statement = connection.prepareStatement(
          "INSERT INTO questions (question, answer) VALUES (?, ?) RETURNING questions.id as id");
      statement.setString(1, question);
      statement.setString(2, answer);

      ResultSet res = statement.executeQuery();
      if (res.next())
      {
        int id = res.getInt("id");
        questions.put(id, new Question(id, question, answer));
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

  @Override public List<Question> getAllQuestions()
  {
    List<Question> list = new ArrayList<>();
    try
    {
      PreparedStatement statement = connection.prepareStatement(
          "SELECT * FROM questions");
      ResultSet res = statement.executeQuery();

      while (res.next())
      {
        int id = res.getInt("id");
        String question = res.getString("question");
        String answer = res.getString("answer");

        Question question1 = new Question(id, question, answer);
        questions.put(id, question1);
        list.add(question1);
      }
      return list;
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
        String question_1 = res.getString("question");
        String answer = res.getString("answer");

        question = new Question(id,  question_1, answer);
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
