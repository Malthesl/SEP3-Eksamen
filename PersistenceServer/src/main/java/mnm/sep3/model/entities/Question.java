package mnm.sep3.model.entities;

public class Question
{
  private int id;
  private int quizId;
  private String title;
  private int index;

  public Question(int id, int quizId, String title, int index)
  {
    this.id = id;
    this.quizId = quizId;
    this.title = title;
    this.index = index;
  }

  public int getId() {
    return id;
  }

  public int getQuizId() {
    return quizId;
  }

  public String getTitle() {
    return title;
  }

  public int getIndex() {
    return index;
  }
}
