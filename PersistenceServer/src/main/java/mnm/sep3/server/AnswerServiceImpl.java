package mnm.sep3.server;

import io.grpc.stub.StreamObserver;
import mnm.sep3.AddAnswerRequest;
import mnm.sep3.AddAnswerResponse;
import mnm.sep3.AnswerDTO;
import mnm.sep3.AnswerServiceGrpc;
import mnm.sep3.DeleteAnswerRequest;
import mnm.sep3.Empty;
import mnm.sep3.GetAllAnswersInQuestionRequest;
import mnm.sep3.GetAllAnswersInQuestionResponse;
import mnm.sep3.GetAnswerRequest;
import mnm.sep3.GetAnswerResponse;
import mnm.sep3.UpdateAnswerRequest;
import mnm.sep3.model.AnswersManager;
import mnm.sep3.model.entities.Answer;

public class AnswerServiceImpl extends AnswerServiceGrpc.AnswerServiceImplBase {
    private final AnswersManager answersManager;

    public AnswerServiceImpl(AnswersManager answersManager) {
        this.answersManager = answersManager;
    }

    @Override
    public void getAnswer(GetAnswerRequest request, StreamObserver<GetAnswerResponse> responseStreamObserver) {
        var res = GetAnswerResponse.newBuilder();

        Answer answer = answersManager.getAnswer(request.getId());
        res.setAnswer(AnswerDTO.newBuilder()
                        .setId(answer.getAnswerId())
                        .setIndex(answer.getIndex())
                        .setQuestionId(answer.getQuestionId())
                        .setTitle(answer.getTitle())
                        .setIsCorrect(answer.isCorrect())
                        .build()
        );

        responseStreamObserver.onNext(res.build());
        responseStreamObserver.onCompleted();
    }

    @Override
    public void addAnswer(AddAnswerRequest request, StreamObserver<AddAnswerResponse> responseStreamObserver) {
        var res = AddAnswerResponse.newBuilder();

        int id = answersManager.addAnswer(request.getQuestionId(), request.getTitle(), request.getIsCorrect(), request.getIndex());

        res.setAnswer(AnswerDTO.newBuilder()
                        .setQuestionId(request.getQuestionId())
                        .setIsCorrect(request.getIsCorrect())
                        .setTitle(request.getTitle())
                        .setIndex(request.getIndex())
                        .setId(id)
                        .build()
        );

        responseStreamObserver.onNext(res.build());
        responseStreamObserver.onCompleted();
    }

    @Override
    public void updateAnswer(UpdateAnswerRequest request, StreamObserver<Empty> emptyStreamObserver) {
        AnswerDTO answerDTO = request.getNewAnswer();
        Answer newAnswer = new Answer(
                answerDTO.getQuestionId(),
                answerDTO.getId(),
                answerDTO.getTitle(),
                answerDTO.getIsCorrect(),
                answerDTO.getIndex()
        );
        answersManager.updateAnswer(newAnswer);

        emptyStreamObserver.onNext(Empty.newBuilder().build());
        emptyStreamObserver.onCompleted();
    }

    @Override
    public void deleteAnswer(DeleteAnswerRequest request, StreamObserver<Empty> emptyStreamObserver) {
        answersManager.deleteAnswer(request.getAnswerId());

        emptyStreamObserver.onNext(Empty.newBuilder().build());
        emptyStreamObserver.onCompleted();
    }

    @Override
    public void getAllAnswersInQuestion(GetAllAnswersInQuestionRequest request, StreamObserver<GetAllAnswersInQuestionResponse> responseStreamObserver) {
        var res = GetAllAnswersInQuestionResponse.newBuilder();

        for (var answer : answersManager.getAllAnswersForQuestion(request.getQuestionId())) {
            res.addAnswers(AnswerDTO.newBuilder()
                            .setId(answer.getAnswerId())
                            .setIndex(answer.getIndex())
                            .setIsCorrect(answer.isCorrect())
                            .setTitle(answer.getTitle())
                            .setQuestionId(answer.getQuestionId())
                    .build());
        }

        responseStreamObserver.onNext(res.build());
        responseStreamObserver.onCompleted();
    }
}
