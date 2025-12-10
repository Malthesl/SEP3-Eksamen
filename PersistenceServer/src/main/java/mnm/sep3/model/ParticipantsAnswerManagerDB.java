package mnm.sep3.model;

import mnm.sep3.database.Database;
import mnm.sep3.model.entities.ParticipantAnswer;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.List;

public class ParticipantsAnswerManagerDB implements ParticipantsAnswerManager {
    private final Connection connection = Database.getConnection();

    @Override
    public ParticipantAnswer addParticipantAnswer(int answerId, int participantId) {
        try {
            PreparedStatement statement = connection.prepareStatement("INSERT INTO participant_answers (answer_id, participant) VALUES (?, ?) RETURNING id");
            statement.setInt(1, answerId);
            statement.setInt(2, participantId);

            ResultSet res = statement.executeQuery();
            if (res.next()) {
                return new ParticipantAnswer(
                        res.getInt("id"),
                        answerId,
                        participantId
                );
            }

            throw new RuntimeException("Kunne ikke indsætte");
        } catch (SQLException e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public List<ParticipantAnswer> getGameResults(String gameId) {
        try {
            PreparedStatement statement = connection.prepareStatement("SELECT pa.id as id, answer_id, participant FROM participant_answers pa INNER JOIN sep3_eksamen.participants p on p.id = pa.participant WHERE p.game_id = ?");
            statement.setString(1, gameId);

            return ConvertResToListOfParticipantAnswers(statement.executeQuery());
        } catch (SQLException e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public List<ParticipantAnswer> getAnswersOnQuestion(String gameId, int questionId) {
        try {
            PreparedStatement statement = connection.prepareStatement("SELECT pa.id as id, answer_id, participant FROM participant_answers pa INNER JOIN sep3_eksamen.participants p on p.id = pa.participant INNER JOIN sep3_eksamen.answers a on pa.answer_id = a.id WHERE p.game_id = ? AND a.question_id = ?");
            statement.setString(1, gameId);
            statement.setInt(2, questionId);

            return ConvertResToListOfParticipantAnswers(statement.executeQuery());
        } catch (SQLException e) {
            throw new RuntimeException(e);
        }
    }

    private List<ParticipantAnswer> ConvertResToListOfParticipantAnswers(ResultSet res) throws SQLException {
        List<ParticipantAnswer> returnList = new ArrayList<>();
        while (res.next()) {
            int id = res.getInt("id");
            int answerId = res.getInt("answer_id");
            int participantId = res.getInt("participant");

            returnList.add(new ParticipantAnswer(id, answerId, participantId));
        }

        return returnList;
    }
}
