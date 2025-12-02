package mnm.sep3.model;

import mnm.sep3.model.entities.Answer;

import java.util.List;

public interface AnswersManager {
    Answer getAnswer(int id);
    int addAnswer(int questionId, String title, boolean isCorrect, int index);
    void updateAnswer(Answer answer);
    void deleteAnswer(int answerId);
    List<Answer> getAllAnswersForQuestion(int questionId);
}
