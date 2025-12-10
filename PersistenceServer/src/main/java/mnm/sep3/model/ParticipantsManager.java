package mnm.sep3.model;

import mnm.sep3.model.entities.Participant;

import java.util.List;

public interface ParticipantsManager {
    /**
     * Persistere en deltager i et afholdt spil
     * @param gameId ID'et på spillet som deltageren deltog i
     * @param name Navnet på spilleren
     * @return Den ny oprettede deltager
     */
    Participant addParticipant(String gameId, String name, int score);

    /**
     * Henter alle deltagere i et spil ud fra ID'et på spillet
     */
    List<Participant> getAllParticipantsInGame(String gameId);
}
