package mnm.sep3.model;

import java.util.List;

public interface QuestionsManager {
    /**
     * @param question Spørgsmålet
     * @param answer Det korrekte svar
     * @return Id'et på det nye spørgsmål
     */
    int addQuestion(String question, String answer);

    /**
     * Henter alle spørgsmål
     */
    List<Question> getAllQuestions();

    /**
     * Henter et spørgsmål ud fra dens id
     */
    Question getQuestionById(int id);
}
