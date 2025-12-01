package mnm.sep3.model;

import mnm.sep3.model.entities.Quiz;

import java.util.List;

public interface QuizzesManager
{
  /**
   *
   * @param quizId id'et på quizzen
   * @return quizzen
   */
  Quiz getQuiz(int quizId);
  /**
   * tilføjer et quiz
   * @param title titlen på quizzet
   * @return navnet på den nye quiz
   */
  int addQuiz(String title);
  /**
   * opdatere quizzet
   */
  void updateQuiz(Quiz quiz);
  /**
   * Sletter et quiz ud fra id'et
   * @param quizId id'et på quiz
   */
  void deleteQuiz(int quizId);
  List<Quiz> queryQuizzes(String title, int byCreator);

}
