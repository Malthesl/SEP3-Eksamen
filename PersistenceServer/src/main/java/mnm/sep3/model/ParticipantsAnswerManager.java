package mnm.sep3.model;

import mnm.sep3.model.entities.ParticipantAnswer;

import java.util.List;

public interface ParticipantsAnswerManager {
    /**
     * Persisterer et svar fra en deltager i et afholdt spil
     * @param answerId ID'et på svaret givet
     * @param participantId ID'et på deltageren i spillet
     * @return Det ny oprettede svar
     */
    ParticipantAnswer AddParticipantAnswer(int answerId, int participantId);

    /**
     * Henter alle svar i et afholdt spil, ud fra spillets ID
     */
    List<ParticipantAnswer> GetGameResults(String gameId);

    /**
     * Henter alle svar på et spørgsmål, i en afholdt quiz
     * @param gameId ID'et på spillet
     * @param questionId ID'et på spørgsmålet
     */
    List<ParticipantAnswer> GetAnswersOnQuestion(String gameId, int questionId);
}
