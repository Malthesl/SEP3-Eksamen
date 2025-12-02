namespace ApiContracts;

public class QuestionDTO
{
    public required int QuestionId { get; set; }
    public required string Title { get; set; }
    public required int QuizId { get; set; }
    public required int Index { get; set; }
}

public class CreateQuestionDTO
{
    public required int QuizId { get; set; }
    public required string Title { get; set; }
}