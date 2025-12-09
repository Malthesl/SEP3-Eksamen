namespace ApiContracts;

public class QuizDTO
{
    public required int Id { get; set; }
    public required string Title { get; set; }
    public required string Visibility { get; set; }
    public required int CreatorId { get; set; }
    public UserDTO? Creator { get; set; }
    public int? QuestionCount { get; set; }
}

public class CreateQuizDTO
{
    public required string Title { get; set; }
}

public class UpdateQuizDTO
{
    public string? Title { get; set; }
    public string? Visibility { get; set; }
}

public class QuizQueryDTO
{
    public required IEnumerable<QuizDTO> Quizzes { get; set; }
    public required int Start { get; set; }
    public required int End { get; set; }
    public required int Count { get; set; }
}