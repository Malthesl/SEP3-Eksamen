package mnm.sep3.model;

import java.util.List;

public interface QuestionsManager
{
  int addQuestion(String question, String answer);
  List<Question> getAllQuestions();
  Question getQuestionById(int id);
}
