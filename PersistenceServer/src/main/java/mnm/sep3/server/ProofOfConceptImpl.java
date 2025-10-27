package mnm.sep3.server;

import io.grpc.stub.StreamObserver;
import mnm.sep3.*;
import mnm.sep3.CreateQuestionRequest;
import mnm.sep3.GetQuestionRequest;
import mnm.sep3.GetQuestionResponse;
import mnm.sep3.GetQuestionsRequest;
import mnm.sep3.GetQuestionsResponse;
import mnm.sep3.ProofOfConceptServiceGrpc;
import mnm.sep3.QuestionDTO;
import mnm.sep3.model.Question;
import mnm.sep3.model.QuestionsManager;

import java.util.List;

public class ProofOfConceptImpl extends ProofOfConceptServiceGrpc.ProofOfConceptServiceImplBase {
    private final QuestionsManager questionsManager;

    public ProofOfConceptImpl(QuestionsManager questionsManager) {
        this.questionsManager = questionsManager;
    }


    @Override
    public void getAllQuestions(GetQuestionsRequest request, StreamObserver<GetQuestionsResponse> responseStreamObserver) {
        GetQuestionsResponse.Builder response = GetQuestionsResponse.newBuilder();

        List<Question> allQuestions = questionsManager.getAllQuestions();

        for (Question question : allQuestions) {
            var questionDTO = QuestionDTO.newBuilder()
                    .setQuestion(question.getQuestion())
                    .setAnswer(question.getAnswer())
                    .setId(question.getId())
                    .build();

            response.addQuestion(questionDTO);
        }

        responseStreamObserver.onNext(response.build());
        responseStreamObserver.onCompleted();
    }

    @Override
    public void getQuestion(GetQuestionRequest request, StreamObserver<GetQuestionResponse> responseStreamObserver) {
        var response = GetQuestionResponse.newBuilder();

        Question question = questionsManager.getQuestionById(request.getQuestionId());

        QuestionDTO Dto = QuestionDTO.newBuilder()
                .setQuestion(question.getQuestion())
                .setAnswer(question.getAnswer())
                .setId(question.getId())
                .build();

        response.setQuestion(Dto);

        responseStreamObserver.onNext(response.build());
        responseStreamObserver.onCompleted();
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
