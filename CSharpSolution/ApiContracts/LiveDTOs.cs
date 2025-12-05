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
    public required long RelTime { get; init; }
    public required long CountdownToTime { get; init; }
    
    public required string GameId { get; init; } = Guid.NewGuid().ToString();
    public required int HostUserId { get; init; }
    public required int QuizId { get; init; }
    public required string JoinCode { get; init; }
    
    public required int CurrentQuestionId { get; set; }

    public required string CurrentState { get; set; }

    public required QuizDTO Quiz { get; init; }

    public required List<LiveGameQuestionDTO> Questions { get; init; }
    
    public required List<LiveGamePlayerDTO> Players { get; init; }
}

public class LiveGamePlayerDTO
{
    public string PlayerId { get; set; }
    public string Name { get; set; }
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

public class LiveGameJoinRequestDTO
{
    public string JoinCode { get; set; }
    public string Name { get; set; }
}

public class LiveGameJoinResponseDTO
{
    public string GameId { get; set; }
    public string PlayerId { get; set; }
}

public class LiveBasicHostRequestDTO
{
    public string GameId { get; set; }
}