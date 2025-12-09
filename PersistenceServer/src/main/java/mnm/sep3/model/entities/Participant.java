package mnm.sep3.model.entities;

public class Participant {
    private final int id;
    private final String gameId;
    private final String name;

    public Participant(int id, String gameId, String name) {
        this.id = id;
        this.gameId = gameId;
        this.name = name;
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
}
