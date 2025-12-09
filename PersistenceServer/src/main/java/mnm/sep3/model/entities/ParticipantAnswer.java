package mnm.sep3.model.entities;

public class ParticipantAnswer {
    private final int id;
    private final int answerId;
    private final int participantId;

    public ParticipantAnswer(int id, int answerId, int participantId) {
        this.id = id;
        this.answerId = answerId;
        this.participantId = participantId;
    }

    public int getId() {
        return id;
    }

    public int getAnswerId() {
        return answerId;
    }

    public int getParticipantId() {
        return participantId;
    }
}
