using ApiContracts;

namespace HostApp.Services;

public class HttpGameResultsService(HttpClient httpClient) : IGameResultsService
{
    public async Task<List<GameResultDTO>> GetGames()
    {
        var response = await httpClient.GetAsync("results");
        if (!response.IsSuccessStatusCode) throw new Exception("Fejl i kald til /results: " + response.StatusCode);

        return (await response.Content.ReadFromJsonAsync<List<GameResultDTO>>())!;
    }

    public async Task<GameResultDTO> GetGame(string gameId)
    {
        var response = await httpClient.GetAsync($"results/{gameId}");
        if (!response.IsSuccessStatusCode) throw new Exception($"Fejl i kald til /results/{gameId}: " + response.StatusCode);

        return (await response.Content.ReadFromJsonAsync<GameResultDTO>())!;
    }

    public async Task<List<GameParticipantDTO>> GetParticipants(string gameId)
    {
        var response = await httpClient.GetAsync($"results/{gameId}/participants");
        if (!response.IsSuccessStatusCode) throw new Exception($"Fejl i kald til /results/{gameId}/participants: " + response.StatusCode);

        return (await response.Content.ReadFromJsonAsync<List<GameParticipantDTO>>())!;
    }

    public async Task<List<GameParticipantAnswerDTO>> GetAnswers(string gameId, int questionId)
    {
        var response = await httpClient.GetAsync($"results/{gameId}/questions/{questionId}/answers");
        if (!response.IsSuccessStatusCode) throw new Exception($"Fejl i kald til /results/{gameId}/questions/{questionId}/answers: " + response.StatusCode);

        return (await response.Content.ReadFromJsonAsync<List<GameParticipantAnswerDTO>>())!;
    }
}