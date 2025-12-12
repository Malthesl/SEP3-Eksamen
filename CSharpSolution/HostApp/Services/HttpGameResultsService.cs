using ApiContracts;
using static HostApp.Utils.Utils;

namespace HostApp.Services;

public class HttpGameResultsService(HttpClient httpClient) : IGameResultsService
{
    public async Task<List<GameResultDTO>> GetGames()
    {
        var response = await httpClient.GetAsync("results");
        await CheckResponse(response);
        
        return (await response.Content.ReadFromJsonAsync<List<GameResultDTO>>())!;
    }

    public async Task<GameResultDTO> GetGame(string gameId)
    {
        var response = await httpClient.GetAsync($"results/{gameId}");
        await CheckResponse(response);
        
        return (await response.Content.ReadFromJsonAsync<GameResultDTO>())!;
    }

    public async Task<List<GameParticipantDTO>> GetParticipants(string gameId)
    {
        var response = await httpClient.GetAsync($"results/{gameId}/participants");
        await CheckResponse(response);
        
        return (await response.Content.ReadFromJsonAsync<List<GameParticipantDTO>>())!;
    }

    public async Task<List<GameParticipantAnswerDTO>> GetAnswers(string gameId, int questionId)
    {
        var response = await httpClient.GetAsync($"results/{gameId}/questions/{questionId}/answers");
        await CheckResponse(response);

        return (await response.Content.ReadFromJsonAsync<List<GameParticipantAnswerDTO>>())!;
    }
}