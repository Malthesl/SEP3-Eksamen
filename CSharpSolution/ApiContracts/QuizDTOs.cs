namespace ApiContracts;

public class QuizDTO
{
    public required int Id { get; init; }
    public required string Title { get; init; }
    public required string Visibility { get; init; }
    public required int CreatorId { get; init; }
}

public class QuizQueryDTO
{
    public required IEnumerable<QuizDTO> Quizzes { get; init; }
    public required int Start { get; init; }
    public required int Count { get; init; }
}