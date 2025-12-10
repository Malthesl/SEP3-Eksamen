namespace ApiContracts;

public class GameResultDTO
{
    public required string GameId { get; set; }
    public required int HostUserId { get; set; }
    public required long PlayedTime { get; set; }
    public required int QuizId { get; set; }
    public required QuizDTO Quiz { get; set; }
}

public class GameParticipantDTO
{
    public required string GameId { get; set; }
    public required int ParticipantId { get; set; }
    public required string ParticipantName { get; set; }
    public required IEnumerable<GameParticipantAnswerDTO> Answers { get; set; }
}

public class GameParticipantAnswerDTO
{
    public required string GameId { get; set; }
    public required int ParticipantId { get; set; }
    public required string ParticipantName { get; set; }
    public required int AnswerId { get; set; }
    public required QuestionDTO Question { get; set; }
    public required IEnumerable<AnswerDTO> Answers { get; set; }
}