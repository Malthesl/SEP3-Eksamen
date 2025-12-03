package mnm.sep3.server;

import io.grpc.stub.StreamObserver;
import mnm.sep3.AddQuizRequest;
import mnm.sep3.AddQuizResponse;
import mnm.sep3.DeleteQuizRequest;
import mnm.sep3.Empty;
import mnm.sep3.GetQuizRequest;
import mnm.sep3.GetQuizResponse;
import mnm.sep3.QueryQuizzesRequest;
import mnm.sep3.QueryQuizzesResponse;
import mnm.sep3.QuizDTO;
import mnm.sep3.QuizServiceGrpc;
import mnm.sep3.UpdateQuizRequest;
import mnm.sep3.UserInfoDTO;
import mnm.sep3.model.QuizzesManager;
import mnm.sep3.model.entities.QueryResult;
import mnm.sep3.model.entities.Quiz;

public class QuizServiceImpl extends QuizServiceGrpc.QuizServiceImplBase {
    private QuizzesManager quizzesManager;

    public QuizServiceImpl(QuizzesManager quizzesManager) {
        this.quizzesManager = quizzesManager;
    }

    @Override
    public void getQuiz(GetQuizRequest request,
                        StreamObserver<GetQuizResponse> responseStreamObserver) {
        GetQuizResponse.Builder response = GetQuizResponse.newBuilder();

        Quiz quiz = quizzesManager.getQuiz(request.getQuizId());

        QuizDTO dto = QuizDTO.newBuilder()
                .setId(request.getQuizId())
                .setTitle(quiz.getTitle())
                .setVisibility(quiz.getVisibility())
                .setCreatorId(quiz.getCreatorId())
                .setCreator(UserInfoDTO.newBuilder()
                        .setId(quiz.getCreatorId())
                        .setUsername(quiz.getCreator().getUsername())
                        .build())
                .setQuestionCount(quiz.getQuestionsCount())
                .build();

        response.setQuiz(dto);

        responseStreamObserver.onNext(response.build());
        responseStreamObserver.onCompleted();
    }

    @Override
    public void addQuiz(AddQuizRequest request,
                        StreamObserver<AddQuizResponse> responseStreamObserver) {
        AddQuizResponse.Builder response = AddQuizResponse.newBuilder();

        int quizId = quizzesManager.addQuiz(request.getTitle(),
                request.getCreatorId());

        QuizDTO dto = QuizDTO.newBuilder()
                .setId(quizId)
                .setTitle(request.getTitle())
                .setVisibility("private")
                .setCreatorId(request.getCreatorId())
                .build();

        response.setQuizDto(dto);

        responseStreamObserver.onNext(response.build());
        responseStreamObserver.onCompleted();
    }

    @Override
    public void updateQuiz(UpdateQuizRequest request, StreamObserver<Empty> responseStreamObserver) {
        UpdateQuizRequest.Builder response = UpdateQuizRequest.newBuilder();
        quizzesManager.updateQuiz(new Quiz(request.getQuiz().getId(), request.getQuiz().getTitle(), request.getQuiz().getVisibility(), request.getQuiz().getCreatorId()));

        QuizDTO dto = QuizDTO.newBuilder()
                .build();

        response.setQuiz(dto);

        responseStreamObserver.onNext(Empty.newBuilder().build());
        responseStreamObserver.onCompleted();
    }

    @Override
    public void deleteQuiz(DeleteQuizRequest request, StreamObserver<Empty> responseStreamObserver) {
        DeleteQuizRequest.Builder response = DeleteQuizRequest.newBuilder();
        quizzesManager.deleteQuiz(request.getQuizId());

        QuizDTO dto = QuizDTO.newBuilder()
                .setId(request.getQuizId())
                .build();

        response.setQuizId(dto.getId());

        responseStreamObserver.onNext(Empty.newBuilder().build());
        responseStreamObserver.onCompleted();
    }

    @Override
    public void queryQuizzes(QueryQuizzesRequest request, StreamObserver<QueryQuizzesResponse> responseObserver) {
        QueryQuizzesResponse.Builder response = QueryQuizzesResponse.newBuilder();

        QueryResult<Quiz> quizzes = quizzesManager.queryQuizzes(request.getSearchQuery(), request.getByCreatorId(), request.getStart(), request.getEnd(), request.getVisibilitiesList());

        response.setStart(request.getStart());
        response.setEnd(request.getStart() + quizzes.results.size());
        response.setCount(quizzes.count);

        response.addAllQuizzes(quizzes.results.stream().map(quiz -> QuizDTO.newBuilder()
                .setId(quiz.getQuizId())
                .setTitle(quiz.getTitle())
                .setVisibility(quiz.getVisibility())
                .setCreatorId(quiz.getCreatorId())
                .setCreator(UserInfoDTO.newBuilder()
                        .setId(quiz.getCreator().getId())
                        .setUsername(quiz.getCreator().getUsername())
                        .build())
                .setQuestionCount(quiz.getQuestionsCount())
                .build()
        ).toList());

        responseObserver.onNext(response.build());
        responseObserver.onCompleted();
    }

}
