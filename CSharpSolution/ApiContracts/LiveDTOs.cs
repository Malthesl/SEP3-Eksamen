namespace ApiContracts;

public class LiveCreateQuizRequestDTO
{
    public int QuizId { get; set; }
}

public class LiveCreateQuizResponseDTO
{
    public string GameId { get; set; }
}

public class LiveGameStatusDTO
{
    public string GameId { get; init; } = Guid.NewGuid().ToString();
    public int HostUserId { get; init; }
    public int QuizId { get; init; }
    public string JoinCode { get; init; }
    
    public int CurrentQuestionId { get; set; }

    public string CurrentState { get; set; }

    public QuizDTO Quiz { get; init; }

    public List<LiveGameQuestionDTO> Questions { get; init; }
}

public class LiveGameQuestionDTO
{
    public required int QuestionId { get; set; }
    public required String Title { get; set; }
    public required List<LiveGameAnswerDTO> Answers { get; set; }
}

public class LiveGameAnswerDTO
{
    public required int AnswerId { get; set; }
    public required String Title { get; set; }
    public required bool IsCorrect { get; set; }
}