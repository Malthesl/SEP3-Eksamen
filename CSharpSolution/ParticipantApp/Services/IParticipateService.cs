namespace ParticipantApp.Services;

public interface IParticipateService
{
    Task<string> JoinAsync(string code, string name);
    Task<string> AnswerAsync(string gameId, string playerId, int questionId, int answerId);
}