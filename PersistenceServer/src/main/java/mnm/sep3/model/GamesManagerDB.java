package mnm.sep3.model;

import mnm.sep3.database.Database;
import mnm.sep3.model.entities.Game;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class GamesManagerDB implements GamesManager {
    private final Connection connection = Database.getConnection();

    private final Map<String, Game> games = new HashMap<>();

    @Override
    public Game addGame(String gameId, int hostId, long playedTime, int quizId) {
        try {
            PreparedStatement statement = connection.prepareStatement("INSERT INTO games (game_id, host_id, played_time, quiz_id) VALUES (?, ?, ? ,?)");
            statement.setString(1, gameId);
            statement.setInt(2, hostId);
            statement.setLong(3, playedTime);
            statement.setInt(4, quizId);

            statement.execute();

            Game returnGame = new Game(gameId, hostId, playedTime, quizId);
            games.put(gameId, returnGame);
            return returnGame;
        } catch (SQLException e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public Game getGame(String gameId) {
        Game returnGame = games.get(gameId);
        if (returnGame != null) return returnGame;

        try {
            PreparedStatement statement = connection.prepareStatement("SELECT * FROM games WHERE game_id = ?");
            statement.setString(1, gameId);

            ResultSet res = statement.executeQuery();

            if (res.next()) {
                int hostId = res.getInt("host_id");
                long playedTime = res.getLong("played_time");
                int quizId = res.getInt("quiz_id");

                returnGame = new Game(gameId, hostId, playedTime, quizId);
                games.put(gameId, returnGame);
                return returnGame;
            }

            throw new RuntimeException("Game not found");
        } catch (SQLException e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public List<Game> getGamesHostedByUser(int hostId) {
        try {
            PreparedStatement statement = connection.prepareStatement("SELECT * FROM games WHERE host_id = ?");
            statement.setInt(1, hostId);

            ResultSet res = statement.executeQuery();

            List<Game> returnList = new ArrayList<>();
            while (res.next()) {
                String gameId = res.getString("game_id");
                long playedTime = res.getLong("played_time");
                int quizId = res.getInt("quiz_id");

                Game fetchedGame = new Game(gameId, hostId, playedTime, quizId);
                games.put(gameId, fetchedGame);
                returnList.add(fetchedGame);
            }

            return returnList;
        } catch (SQLException e) {
            throw new RuntimeException(e);
        }
    }
}
