package mnm.sep3.model;

import mnm.sep3.model.entities.Game;

import java.util.List;

public interface GamesManager {
    /**
     * Persistere et afholdt spil
     * @param gameId ID'et på spillet afholdt, string på 36 karakterer
     * @param hostId ID'et på værtens bruger
     * @param playedTime Tidspunktet spillet blev afholdt i unix tid
     * @param quizId ID'et på quizzen
     * @return Det ny oprettede spil
     */
    Game addGame(String gameId, int hostId, long playedTime, int quizId);

    /**
     * Henter et spil ud fra spillets ID
     */
    Game getGame(String gameId);

    /**
     * Henter alle spillende som en vært har afholdt
     * @param hostId ID'et på værten
     */
    List<Game> getGamesHostedByUser(int hostId);
}
