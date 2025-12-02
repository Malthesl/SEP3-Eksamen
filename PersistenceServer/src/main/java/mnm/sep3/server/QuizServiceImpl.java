package mnm.sep3.server;

import io.grpc.stub.StreamObserver;
import mnm.sep3.*;
import mnm.sep3.model.QuizzesManager;
import mnm.sep3.model.entities.Quiz;

public class QuizServiceImpl extends QuizzesServiceGrpc.QuizzesServiceImplBase
{
  private QuizzesManager quizzesManager;

  public QuizServiceImpl(QuizzesManager quizzesManager)
  {
    this.quizzesManager = quizzesManager;
  }

  public void getQuiz(GetQuizRequest request,
      StreamObserver<GetQuizResponse> responseStreamObserver)
  {
    GetQuizResponse.Builder response = GetQuizResponse.newBuilder();

    Quiz quiz = quizzesManager.getQuiz(request.getQuizId());

    QuizDTO dto = QuizDTO.newBuilder()
        .setId(request.getQuizId())
        .setTitle(quiz.getTitle())
        .setVisibility(quiz.getVisibility())
        .setCreatorId(quiz.getCreatorId())
        .build();

    response.setQuiz(dto);

    responseStreamObserver.onNext(response.build());
    responseStreamObserver.onCompleted();
  }

  public void addQuiz(AddQuizRequest request,
      StreamObserver<AddQuizResponse> responseStreamObserver)
  {
    AddQuizResponse.Builder response = AddQuizResponse.newBuilder();

    int quizId = quizzesManager.addQuiz(request.getTitle(),
        request.getCreatorId());

    QuizDTO dto = QuizDTO.newBuilder()
        .setId(quizId)
        .setTitle(request.getTitle())
        .setVisibility("private")
        .setCreatorId(request.getCreatorId())
        .build();

    response.setCreatorId(dto.getCreatorId());

    responseStreamObserver.onNext(response.build());
    responseStreamObserver.onCompleted();
  }

  public void updateQuiz (UpdateQuizRequest request, StreamObserver<Empty> responseStreamObserver){
    UpdateQuizRequest.Builder response = UpdateQuizRequest.newBuilder();
    quizzesManager.updateQuiz(new Quiz(request.getQuiz().getId(),request.getQuiz().getTitle(),request.getQuiz().getVisibility(),request.getQuiz().getCreatorId()));

    QuizDTO dto = QuizDTO.newBuilder()
        .build();

    response.setQuiz(dto);

    responseStreamObserver.onNext(Empty.newBuilder().build());
    responseStreamObserver.onCompleted();
  }

  public void DeleteQuiz(DeleteQuizRequest request, StreamObserver<Empty> responseStreamObserver)
  {
    DeleteQuizRequest.Builder response = DeleteQuizRequest.newBuilder();
    quizzesManager.deleteQuiz(request.getQuizId());

    QuizDTO dto = QuizDTO.newBuilder()
        .setId(request.getQuizId())
        .build();

    response.setQuizId(dto.getId());

    responseStreamObserver.onNext(Empty.newBuilder().build());
    responseStreamObserver.onCompleted();
  }

}
