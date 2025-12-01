package mnm.sep3.server;

import io.grpc.Status;
import io.grpc.stub.StreamObserver;
import mnm.sep3.CreateQuestionRequest;
import mnm.sep3.GetQuestionRequest;
import mnm.sep3.GetQuestionResponse;
import mnm.sep3.GetQuestionsRequest;
import mnm.sep3.GetQuestionsResponse;
import mnm.sep3.QuestionDTO;
import mnm.sep3.QuizServiceGrpc;
import mnm.sep3.model.Question;
import mnm.sep3.model.QuestionsManager;

import java.util.List;

public class QuizServiceImpl extends QuizServiceGrpc.QuizServiceImplBase {
    private final QuestionsManager questionsManager;

    public QuizServiceImpl(QuestionsManager questionsManager) {
        this.questionsManager = questionsManager;
    }

    @Override
    public void getQuestion(GetQuestionRequest request, StreamObserver<GetQuestionResponse> responseStreamObserver) {

    }

    @Override
    public void getAllQuestions(GetQuestionsRequest request, StreamObserver<GetQuestionsResponse> responseStreamObserver) {

    }

    @Override
    public void createQuestion(CreateQuestionRequest request, StreamObserver<GetQuestionResponse> responseStreamObserver) {

    }
}
