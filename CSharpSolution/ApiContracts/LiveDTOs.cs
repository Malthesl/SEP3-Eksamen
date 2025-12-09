namespace ApiContracts;

public class LiveCreateGameRequestDTO
{
    public required int QuizId { get; set; }
}

public class LiveCreateGameResponseDTO
{
    public required string GameId { get; set; }
}

public class LiveGameStatusDTO
{
    public required int UpdateNo { get; set; }
    public required long RelTime { get; set; }
    public required long CountdownToTime { get; set; }
    public required string GameId { get; set; }
    public required int HostUserId { get; set; }
    public required int QuizId { get; set; }
    public required string CurrentState { get; set; }
    public required int CurrentQuestionId { get; set; }
    
}

public class LiveGameHostStatusDTO : LiveGameStatusDTO
{
    public required string JoinCode { get; set; }
    public required QuizDTO Quiz { get; set; }
    public required List<LiveGameQuestionDTO> Questions { get; set; }
    public required List<LiveGamePlayerDTO> Players { get; set; }
    public required int PlayersAnswered { get; set; } 
}

public class LiveGamePlayerStatusDTO : LiveGameStatusDTO
{
    public required List<LiveGameQuestionCensoredDTO> Questions { get; set; }
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