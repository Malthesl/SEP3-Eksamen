package mnm.sep3.model.entities;

public class Game {
    private final String gameId;
    private final int hostId;
    private final long playedTime;
    private final int quizId;

    public Game(String gameId, int hostId, long playedTime, int quizId) {
        this.gameId = gameId;
        this.hostId = hostId;
        this.playedTime = playedTime;
        this.quizId = quizId;
    }

    public String getGameId() {
        return gameId;
    }

    public int getHostId() {
        return hostId;
    }

    public long getPlayedTime() {
        return playedTime;
    }

    public int getQuizId() {
        return quizId;
    }
}
