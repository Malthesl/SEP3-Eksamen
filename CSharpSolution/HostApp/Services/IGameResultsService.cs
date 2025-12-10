using ApiContracts;

namespace HostApp.Services;

public interface IGameResultsService
{
    Task<List<GameResultDTO>> GetGames();
    Task<GameResultDTO> GetGame(string gameId);
    Task<List<GameParticipantDTO>> GetParticipants(string gameId);
    Task<List<GameParticipantAnswerDTO>> GetAnswers(string gameId, int questionId);
}