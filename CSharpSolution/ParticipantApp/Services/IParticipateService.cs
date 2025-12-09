using ApiContracts;

namespace ParticipantApp.Services;

public interface IParticipateService
{
    Task<string> JoinAsync(string code, string name);
    Task AnswerAsync(int questionId, int answerId);
    Task<LiveGamePlayerStatusDTO> GetGameStatusAsync(int updateNo = 0);
}