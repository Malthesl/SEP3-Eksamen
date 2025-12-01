package mnm.sep3.model.entities;

public class Answer {
    private int questionId;
    private int answerId;
    private String title;
    private boolean correct;
    private int index;

    public Answer(int questionId, int answerId, String title, boolean correct, int index) {
        this.questionId = questionId;
        this.answerId = answerId;
        this.title = title;
        this.correct = correct;
        this.index = index;
    }

    public int getAnswerId() {
        return answerId;
    }

    public String getTitle() {
        return title;
    }

    public boolean isCorrect() {
        return correct;
    }

    public int getIndex() {
        return index;
    }

    public int getQuestionId() {
        return questionId;
    }
}
