package mnm.sep3.server;

import io.grpc.stub.StreamObserver;
import mnm.sep3.*;
import mnm.sep3.model.GamesManager;
import mnm.sep3.model.ParticipantsAnswerManager;
import mnm.sep3.model.ParticipantsManager;
import mnm.sep3.model.entities.Game;

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

    @Override
    public void getGamesHostedByUser(GetGamesHostedByUserRequest request, StreamObserver<GetGamesHostedByUserResponse> responseStreamObserver) {
        var res = GetGamesHostedByUserResponse.newBuilder();

        int userId = request.getUserId();
        for (var game : gamesManager.getGamesHostedByUser(userId)) {
            res.addGames(GameDTO.newBuilder()
                            .setHostId(game.getHostId())
                            .setId(game.getGameId())
                            .setPlayedTime(game.getPlayedTime())
                            .setQuizId(game.getQuizId())
                            .build()
            );
        }

        responseStreamObserver.onNext(res.build());
        responseStreamObserver.onCompleted();
    }

    @Override
    public void getGame(GetGameRequest request, StreamObserver<GetGameResponse> responseStreamObserver) {
        var res = GetGameResponse.newBuilder();

        Game game = gamesManager.getGame(request.getGameId());

        GameDTO gameDto = GameDTO.newBuilder()
                .setQuizId(game.getQuizId())
                .setPlayedTime(game.getPlayedTime())
                .setId(game.getGameId())
                .setHostId(game.getHostId())
                .build();

        res.setGame(gameDto);

        responseStreamObserver.onNext(res.build());
        responseStreamObserver.onCompleted();
    }

  @Override public void getParticipantsInGame(GetParticipantsInGameRequest request,
      StreamObserver<GetParticipantsInGameResponse> responseStreamObserver)
  {
      var res = GetParticipantsInGameResponse.newBuilder();

      String gameId = request.getGameId();
      for (var participant : participantsManager.GetAllParticipantsInGame(gameId)) {
        res.addParticipants(ParticipantDTO.newBuilder()
                .setGameId(gameId)
                .setId(participant.getId())
                .setName(participant.getName())
                .build());
      }

      responseStreamObserver.onNext(res.build());
      responseStreamObserver.onCompleted();
  }

  @Override
  public void getGameResults (GetGameResultsRequest request, StreamObserver<GetGameResultsResponse> responseStreamObserver){
      var res = GetGameResultsResponse.newBuilder();

      String gameId = request.getGameId();
      for (var gameResult : participantsAnswerManager.GetGameResults(gameId)) {
        res.addAnswers(ParticipantAnswerDTO.newBuilder()
                .setId(gameResult.getId())
                .setAnswer(gameResult.getAnswerId())
                .setParticipantId(gameResult.getParticipantId())
                .build());
      }

      responseStreamObserver.onNext(res.build());
      responseStreamObserver.onCompleted();
  }

  public void getResultOnQuestion(GetResultOnQuestionRequest request,
      StreamObserver<GetResultOnQuestionResponse> responseStreamObserver)
  {
    var res = GetResultOnQuestionResponse.newBuilder();

    String gameId = request.getGameId();
    int questionId = request.getQuestionId();
    for (var resultOnQues : participantsAnswerManager.GetAnswersOnQuestion(gameId, questionId)) {
        res.addAnswers(ParticipantAnswerDTO.newBuilder()
            .setId(resultOnQues.getId())
                .setAnswer(resultOnQues.getAnswerId())
                .setParticipantId(resultOnQues.getParticipantId())
                .build());
    }

    responseStreamObserver.onNext(res.build());
    responseStreamObserver.onCompleted();
  }
}
