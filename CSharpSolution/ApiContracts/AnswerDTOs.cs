namespace ApiContracts;

public class AnswerDTO
{
    public required int AnswerId { get; set; }
    public required string Title { get; set; }
    public required int Index { get; set; }
    public required int QuestionId { get; set; }
    public required bool IsCorrect { get; set; }
}

public class CreateAnswerDTO
{
    public required string Title { get; set; }
    public required int Index { get; set; }
    public required int QuestionId { get; set; }
    public required bool IsCorrect { get; set; }
}