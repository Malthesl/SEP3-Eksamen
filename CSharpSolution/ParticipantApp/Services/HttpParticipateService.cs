using ApiContracts;

namespace ParticipantApp.Services;

public class HttpParticipateService(HttpClient httpClient, IPlayerService playerService) : IParticipateService
{
    public async Task<string> JoinAsync(string code, string name)
    {
        var res = await httpClient.PostAsJsonAsync("/live/join", new LiveGameJoinRequestDTO()
        {
            Name = name,
            JoinCode = code
        });
        
        if (!res.IsSuccessStatusCode) throw new HttpRequestException(res.ReasonPhrase);
        
        var content = (await res.Content.ReadFromJsonAsync<LiveGameJoinResponseDTO>())!;
        
        await playerService.Set(content.PlayerId, content.GameId);
        
        return content.GameId;
    }

    public Task<string> AnswerAsync(string gameId, string playerId, int questionId, int answerId)
    {
        throw new NotImplementedException();
    }
}