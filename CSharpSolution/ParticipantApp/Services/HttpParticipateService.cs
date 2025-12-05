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

    public async Task AnswerAsync(int questionId, int answerId)
    {
        string gameId = (await playerService.GetGameIdAsync())!;
        string playerId = (await playerService.GetPlayerIdAsync())!;

        var req = new LiveGameAnswerRequestDTO
        {
            GameId = gameId,
            AnswerId = answerId,
            PlayerId = playerId,
            QuestionId = questionId
        };
        
        var res = await httpClient.PostAsJsonAsync("/live/answer", req);
        if (!res.IsSuccessStatusCode) throw new HttpRequestException(res.ReasonPhrase);
    }

    public async Task<LiveGamePlayerStatusDTO> GetGameStatusAsync(int updateNo = 0)
    {
        string gameId = (await playerService.GetGameIdAsync())!;
        string playerId = (await playerService.GetPlayerIdAsync())!;
        
        var res = await httpClient.GetAsync($"/live/status?gameId={gameId}&playerId={playerId}&lastUpdateNo={updateNo}");
        if (!res.IsSuccessStatusCode) throw new HttpRequestException(res.ReasonPhrase);
        
        var content = (await res.Content.ReadFromJsonAsync<LiveGamePlayerStatusDTO>())!;
        return content;
    }
}