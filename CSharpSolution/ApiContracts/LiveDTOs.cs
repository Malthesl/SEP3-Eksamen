namespace ApiContracts;

public class LiveCreateQuizRequestDTO
{
    public int QuizId { get; set; }
}

public class LiveCreateQuizResponseDTO
{
    public string GameId { get; set; }
}

public class LiveGameHostStatusDTO
{
    public required int UpdateNo { get; init; }
    
    public required long RelTime { get; init; }
    public required long CountdownToTime { get; init; }
    
    public required string GameId { get; init; }
    public required int HostUserId { get; init; }
    public required int QuizId { get; init; }
    public required string JoinCode { get; init; }
    
    public required string CurrentState { get; set; }
    public required int CurrentQuestionId { get; set; }

    public required QuizDTO Quiz { get; init; }

    public required List<LiveGameQuestionDTO> Questions { get; init; }
    
    public required List<LiveGamePlayerDTO> Players { get; init; }
    
    public required int PlayersAnswered { get; init; } 
}

public class LiveGamePlayerStatusDTO
{
    public required int UpdateNo { get; init; }
    
    public required long RelTime { get; init; }
    public required long CountdownToTime { get; init; }
    
    public required string GameId { get; init; }
    public required int HostUserId { get; init; }
    public required int QuizId { get; init; }

    public required int CurrentQuestionId { get; set; }
    public required string CurrentState { get; set; }

    public required List<LiveGameQuestionCensoredDTO> Questions { get; init; }
    
    public required string PlayerId { get; set; }
    public required string Name { get; set; }
    public required int Score { get; set; }
    public required int Ranking { get; set; }
    public int? LatestScoreChange { get; set; }
    public int? LatestAnswerId { get; set; }
    public bool? LatestAnswerCorrect { get; set; }
}

public class LiveGamePlayerDTO
{
    public required string PlayerId { get; set; }
    public required string Name { get; set; }
    public required int Score { get; set; }
    public int? LatestScoreChange { get; set; }
    public int? LatestAnswerId { get; set; }
    public bool? LatestAnswerCorrect { get; set; }
}

public class LiveGameQuestionCensoredDTO
{
    public required int QuestionId { get; set; }
    public required String Title { get; set; }
    public required List<LiveGameAnswerCensoredDTO> Answers { get; set; }
}

public class LiveGameAnswerCensoredDTO
{
    public required int AnswerId { get; set; }
    public required String Title { get; set; }
    public required int Index { get; set; }
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
    public required int Index { get; set; }
}

public class LiveGameJoinRequestDTO
{
    public required string JoinCode { get; set; }
    public required string Name { get; set; }
}

public class LiveGameJoinResponseDTO
{
    public required string GameId { get; set; }
    public required string PlayerId { get; set; }
}

public class LiveBasicHostRequestDTO
{
    public required string GameId { get; set; }
}

public class LiveGameAnswerRequestDTO
{
    public required string GameId { get; set; }
    public required string PlayerId { get; set; }
    public required int QuestionId { get; set; }
    public required int AnswerId { get; set; }
} 