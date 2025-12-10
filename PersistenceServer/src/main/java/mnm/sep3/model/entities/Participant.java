package mnm.sep3.model.entities;

public class Participant {
    private final int id;
    private final String gameId;
    private final String name;
    private final int score;

    public Participant(int id, String gameId, String name, int score) {
        this.id = id;
        this.gameId = gameId;
        this.name = name;
        this.score = score;
    }

    public int getId() {
        return id;
    }

    public String getGameId() {
        return gameId;
    }

    public String getName() {
        return name;
    }

    public int getScore() {
        return score;
    }
}
