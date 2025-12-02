namespace ApiContracts;

public class QuizDTO
{
    public required int Id { get; init; }
    public required string Title { get; init; }
    public required string Visibility { get; init; }
    public required int CreatorId { get; init; }
    public UserDTO? Creator { get; init; }
    public int? QuestionCount { get; init; }
}

public class CreateQuizDTO
{
    public required string Title { get; set; }
    public required string Visibility { get; set; }
}

public class UpdateQuizDTO
{
    public string? Title { get; set; }
    public string? Visibility { get; set; }
    public int? quizId { get; set; }
}
public class QuizQueryDTO
{
    public required IEnumerable<QuizDTO> Quizzes { get; init; }
    public required int Start { get; init; }
    public required int End { get; init; }
    public required int Count { get; init; }
}