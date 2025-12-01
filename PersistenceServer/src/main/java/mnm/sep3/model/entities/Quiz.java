package mnm.sep3.model.entities;

public class Quiz {
    private int quizId;
    private String title;
    private String visibility;
    private int creatorId;

    public Quiz(int quizId, String title, String visibility, int creatorId) {
        this.quizId = quizId;
        this.title = title;
        this.visibility = visibility;
        this.creatorId = creatorId;
    }

    public int getQuizId() {
        return quizId;
    }

    public String getTitle() {
        return title;
    }

    public String getVisibility() {
        return visibility;
    }

    public int getCreatorId() {
        return creatorId;
    }
}
