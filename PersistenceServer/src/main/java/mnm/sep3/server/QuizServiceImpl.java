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
        try {
            GetQuestionResponse.Builder res = GetQuestionResponse.newBuilder();

            int questionId = request.getQuestionId();

            Question returnQuestion = questionsManager.getQuestionById(questionId);

            QuestionDTO dto = QuestionDTO.newBuilder()
                    .setId(returnQuestion.getId())
                    .setQuestion(returnQuestion.getQuestion())
                    .setAnswer(returnQuestion.getAnswer())
                    .build();

            res.setQuestion(dto);


            responseStreamObserver.onNext(res.build());
            responseStreamObserver.onCompleted();
        } catch (Exception e) {
            e.printStackTrace();
            responseStreamObserver.onError(Status.NOT_FOUND
                    .withDescription("En fejl skete")
                    .augmentDescription(e.getMessage())
                    .asRuntimeException());
        }
    }

    @Override
    public void getAllQuestions(GetQuestionsRequest request, StreamObserver<GetQuestionsResponse> responseStreamObserver) {
        try {
            GetQuestionsResponse.Builder res = GetQuestionsResponse.newBuilder();

            List<Question> questions = questionsManager.getAllQuestions();

            for (var question : questions) {
                res.addQuestion(QuestionDTO.newBuilder()
                        .setAnswer(question.getAnswer())
                        .setQuestion(question.getQuestion())
                        .setId(question.getId()).build());
            }

            responseStreamObserver.onNext(res.build());
            responseStreamObserver.onCompleted();
        } catch (Exception e) {
            e.printStackTrace();
            responseStreamObserver.onError(Status.NOT_FOUND
                    .withDescription("En fejl skete")
                    .augmentDescription(e.getMessage())
                    .asRuntimeException());
        }
    }

    @Override
    public void createQuestion(CreateQuestionRequest request, StreamObserver<GetQuestionResponse> responseStreamObserver) {
        var response = GetQuestionResponse.newBuilder();

        int questionId = questionsManager.addQuestion(request.getQuestion(), request.getAnswer());
        Question question = questionsManager.getQuestionById(questionId);

        QuestionDTO Dto = QuestionDTO.newBuilder()
                .setQuestion(question.getQuestion())
                .setAnswer(question.getAnswer())
                .setId(question.getId())
                .build();

        response.setQuestion(Dto);

        responseStreamObserver.onNext(response.build());
        responseStreamObserver.onCompleted();
    }
}
