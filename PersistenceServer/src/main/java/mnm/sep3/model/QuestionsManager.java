package mnm.sep3.model;

import java.util.List;

public interface QuestionsManager {
    /**
     * @param quizId ID'et på quizzen
     * @param title Titlen på spørgsmålet
     * @return ID'et på det nye spørgsmål
     */
    int addQuestion(int quizId, String title);

    /**
     * Opdatere et spørgsmålet
     */
    void updateQuestion(Question question);

    /**
     * Flytter et spørgsmål til et nyt index. Fx: '1' kommer først, '2' kommer derefter osv.
     * @param questionId ID'et på spørgsmålet
     * @param toIndex Indekset på den nye plads.
     */
    void moveQuestion(int questionId, int toIndex);

    /**
     * Sletter et spørgsmål ud fra spørgsmålets ID
     */
    void deleteQuestion(int questionId);

    /**
     * Henter alle spørgsmål i en quiz
     * @param quizId ID'et på quizzen
     */
    List<Question> getAllQuestionsInQuiz(int quizId);

    /**
     * Henter et spørgsmål ud fra spørgsmålets ID
     */
    Question getQuestionById(int id);
}
