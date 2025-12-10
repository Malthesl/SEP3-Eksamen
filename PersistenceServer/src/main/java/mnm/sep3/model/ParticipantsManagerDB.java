package mnm.sep3.model;

import mnm.sep3.database.Database;
import mnm.sep3.model.entities.Participant;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.List;

public class ParticipantsManagerDB implements ParticipantsManager {
    private final Connection connection = Database.getConnection();

    @Override
    public Participant addParticipant(String gameId, String name, int score) {
        try {
            PreparedStatement statement = connection.prepareStatement("INSERT INTO participants (name, game_id, score) VALUES (?, ?, ?) returning id");
            statement.setString(1, name);
            statement.setString(2, gameId);

            ResultSet res = statement.executeQuery();

            if (res.next()) {
                int id = res.getInt("id");
                return new Participant(id, gameId, name, score);
            }

            throw new RuntimeException("Kunne ikke indsætte");
        } catch (SQLException e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public List<Participant> getAllParticipantsInGame(String gameId) {
        try {
            PreparedStatement statement = connection.prepareStatement("SELECT * FROM participants WHERE game_id = ?");
            statement.setString(1, gameId);

            ResultSet res = statement.executeQuery();

            List<Participant> returnList = new ArrayList<>();
            while (res.next()) {
                returnList.add(new Participant(
                        res.getInt("id"),
                        gameId,
                        res.getString("name"),
                        res.getInt("score")
                ));
            }

            return returnList;
        } catch (SQLException e) {
            throw new RuntimeException(e);
        }
    }
}
