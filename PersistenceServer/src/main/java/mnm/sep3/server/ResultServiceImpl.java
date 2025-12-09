package mnm.sep3.server;

import io.grpc.stub.StreamObserver;
import mnm.sep3.*;
import mnm.sep3.model.GamesManager;
import mnm.sep3.model.ParticipantsAnswerManager;
import mnm.sep3.model.ParticipantsManager;

public class ResultServiceImpl extends ResultServiceGrpc.ResultServiceImplBase {
    private final GamesManager gamesManager;
    private final ParticipantsManager participantsManager;
    private final ParticipantsAnswerManager participantsAnswerManager;

    public ResultServiceImpl(GamesManager gamesManager, ParticipantsManager participantsManager, ParticipantsAnswerManager participantsAnswerManager) {
        this.gamesManager = gamesManager;
        this.participantsManager = participantsManager;
        this.participantsAnswerManager = participantsAnswerManager;
    }

    @Override
    public void submitGameResults(SubmitGameResultsRequest request, StreamObserver<Empty> emptyStreamObserver) {
        GameDTO gameDTO = request.getGame();
        gamesManager.addGame(
                gameDTO.getId(),
                gameDTO.getHostId(),
                gameDTO.getPlayedTime(),
                gameDTO.getQuizId()
        );

        for (var participant : request.getResultsList()) {
            int participantId = participantsManager.AddParticipant(gameDTO.getId(), participant.getName()).getId();

            for (var answer : participant.getAnswersList()) {
                participantsAnswerManager.AddParticipantAnswer(
                        answer.getAnswerId(),
                        participantId
                );
            }
        }

        emptyStreamObserver.onNext(Empty.newBuilder().build());
        emptyStreamObserver.onCompleted();
    }
}
