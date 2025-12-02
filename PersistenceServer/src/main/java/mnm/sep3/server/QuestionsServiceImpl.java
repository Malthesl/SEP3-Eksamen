package mnm.sep3.server;

import io.grpc.stub.StreamObserver;
import mnm.sep3.AddQuestionRequest;
import mnm.sep3.AddQuestionResponse;
import mnm.sep3.DeleteQuestionRequest;
import mnm.sep3.Empty;
import mnm.sep3.GetAllQuestionsInQuizRequest;
import mnm.sep3.GetAllQuestionsInQuizResponse;
import mnm.sep3.GetQuestionByIdRequest;
import mnm.sep3.GetQuestionByIdResponse;
import mnm.sep3.MoveQuestionRequest;
import mnm.sep3.QuestionDTO;
import mnm.sep3.QuestionServiceGrpc;
import mnm.sep3.UpdateQuestionRequest;
import mnm.sep3.model.QuestionsManager;
import mnm.sep3.model.entities.Question;

public class QuestionsServiceImpl extends QuestionServiceGrpc.QuestionServiceImplBase {
    private final QuestionsManager questionsManager;

    public QuestionsServiceImpl(QuestionsManager questionsManager) {
        this.questionsManager = questionsManager;
    }

    @Override
    public void getQuestionById(GetQuestionByIdRequest request, StreamObserver<GetQuestionByIdResponse> responseStreamObserver) {
        var res = GetQuestionByIdResponse.newBuilder();

        Question returnQuestion = questionsManager.getQuestionById(request.getQuestionId());

        QuestionDTO returnDto = QuestionDTO.newBuilder()
                .setId(returnQuestion.getId())
                .setTitle(returnQuestion.getTitle())
                .setIndex(returnQuestion.getIndex())
                .setQuizId(returnQuestion.getQuizId())
                .build();

        res.setQuestion(returnDto);

        responseStreamObserver.onNext(res.build());
        responseStreamObserver.onCompleted();
    }

    @Override
    public void addQuestion(AddQuestionRequest request, StreamObserver<AddQuestionResponse> responseStreamObserver) {
        var res = AddQuestionResponse.newBuilder();

        int questionId = questionsManager.addQuestion(request.getQuizId(), request.getTitle());
        Question returnQuestion = questionsManager.getQuestionById(questionId);

        QuestionDTO returnDto = QuestionDTO.newBuilder()
                .setId(returnQuestion.getId())
                .setTitle(returnQuestion.getTitle())
                .setIndex(returnQuestion.getIndex())
                .setQuizId(returnQuestion.getQuizId())
                .build();

        res.setNewQuestion(returnDto);

        responseStreamObserver.onNext(res.build());
        responseStreamObserver.onCompleted();
    }

    @Override
    public void updateQuestion(UpdateQuestionRequest request, StreamObserver<Empty> emptyStreamObserver) {
        QuestionDTO updatedDto = request.getUpdatedQuestion();
        Question updatedQuestion = new Question(updatedDto.getId(), updatedDto.getQuizId(), updatedDto.getTitle(), updatedDto.getIndex());

        questionsManager.updateQuestion(updatedQuestion);

        emptyStreamObserver.onNext(Empty.newBuilder().build());
        emptyStreamObserver.onCompleted();
    }

    @Override
    public void moveQuestion(MoveQuestionRequest request, StreamObserver<Empty> emptyStreamObserver) {
        questionsManager.moveQuestion(request.getQuestionId(), request.getToIndex());

        emptyStreamObserver.onNext(Empty.newBuilder().build());
        emptyStreamObserver.onCompleted();
    }

    @Override
    public void deleteQuestion(DeleteQuestionRequest request, StreamObserver<Empty> emptyStreamObserver) {
        questionsManager.deleteQuestion(request.getQuestionId());

        emptyStreamObserver.onNext(Empty.newBuilder().build());
        emptyStreamObserver.onCompleted();
    }

    @Override
    public void getAllQuestionsInQuiz(GetAllQuestionsInQuizRequest request, StreamObserver<GetAllQuestionsInQuizResponse> responseStreamObserver) {
        var res = GetAllQuestionsInQuizResponse.newBuilder();

        for (Question question : questionsManager.getAllQuestionsInQuiz(request.getQuizId())) {
            res.addQuestions(QuestionDTO.newBuilder()
                            .setQuizId(question.getQuizId())
                            .setId(question.getId())
                            .setIndex(question.getIndex())
                            .setTitle(question.getTitle())
                            .build());
        }

        responseStreamObserver.onNext(res.build());
        responseStreamObserver.onCompleted();
    }
}
