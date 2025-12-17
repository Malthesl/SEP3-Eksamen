using ApiContracts;
using static HostApp.Utils.Utils;

namespace HostApp.Services;

public class HttpQuizHostService(HttpClient httpClient) : IQuizHostService
{
    public async Task<string> HostQuiz(int quizId)
    {
        var res = await httpClient.PostAsJsonAsync($"/live/new", new LiveCreateGameRequestDTO
        {
            QuizId = quizId
        });
        await CheckResponse(res);

        var content = (await res.Content.ReadFromJsonAsync<LiveCreateGameResponseDTO>())!;
        return content.GameId;
    }

    public async Task<LiveGameHostStatusDTO> GetGameInfo(string gameId, int lastUpdateNo = 0)
    {
        var res = await httpClient.GetAsync($"/live/status?gameId={gameId}&lastUpdateNo={lastUpdateNo}");
        await CheckResponse(res);
        
        var content = (await res.Content.ReadFromJsonAsync<LiveGameHostStatusDTO>())!;
        return content;
    }

    public async Task StartQuiz(string gameId)
    {
        var res = await httpClient.PostAsJsonAsync($"/live/start", new LiveBasicHostRequestDTO
        {
            GameId = gameId
        });

        await CheckResponse(res);
    }

    public async Task ContinueQuiz(string gameId)
    {
        var res = await httpClient.PostAsJsonAsync($"/live/continue", new LiveBasicHostRequestDTO
        {
            GameId = gameId
        });

        await CheckResponse(res);
    }
}