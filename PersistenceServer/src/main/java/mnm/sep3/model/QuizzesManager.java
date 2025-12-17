package mnm.sep3.model;

import mnm.sep3.model.entities.Quiz;

import java.util.List;

public interface QuizzesManager {
    /**
     * @param quizId id'et på quizzen
     * @return quizzen
     */
    Quiz getQuiz(int quizId);

    /**
     * tilføjer et quiz
     *
     * @param title titlen på quizzet
     * @return navnet på den nye quiz
     */
    int addQuiz(String title, int creatorId);

    /**
     * opdatere quizzet
     */
    void updateQuiz(Quiz quiz);

    /**
     * Sletter et quiz ud fra id'et
     *
     * @param quizId id'et på quiz
     */
    void deleteQuiz(int quizId);


    /**
     * Query quizzer
     * @param searchQuery Søgeord at søge efter.
     * @param byCreator Find kun quizzer af denne bruger.
     * @param start Spring til række nr i resultaterne.
     * @param end Returner kun resultater til række nr i resultaterne.
     * @param visibilities En liste af visibility typer at returner.
     *                     En tom liste returner kun offentlige quizzer.
     *                     For at returner private quizzer, skal en byCreator inkluderes.
     */
    QueryResult<Quiz> queryQuizzes(String searchQuery, int byCreator, int start, int end, List<String> visibilities);
}
