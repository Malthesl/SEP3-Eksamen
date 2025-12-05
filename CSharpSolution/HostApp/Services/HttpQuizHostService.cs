using ApiContracts;

namespace HostApp.Services;

public class HttpQuizHostService(HttpClient httpClient) : IQuizHostService
{
    public async Task<string> HostQuiz(int quizId)
    {
        var res = await httpClient.PostAsJsonAsync($"/live/new", new LiveCreateQuizRequestDTO
        {
            QuizId = quizId
        });
        if (!res.IsSuccessStatusCode)
            throw new HttpRequestException($"Http status code {(int)res.StatusCode}: {res.ReasonPhrase}");

        var content = (await res.Content.ReadFromJsonAsync<LiveCreateQuizResponseDTO>())!;
        return content.GameId;
    }

    public async Task<LiveGameStatusDTO> GetGameInfo(string gameId, bool force = false)
    {
        var res = await httpClient.GetAsync($"/live/status?gameId={gameId}&force={force}");

        if (!res.IsSuccessStatusCode)
        {
            Console.WriteLine(await res.Content.ReadAsStringAsync());
            throw new HttpRequestException($"Http status code {(int)res.StatusCode}: {res.ReasonPhrase}");
        }
        var content = (await res.Content.ReadFromJsonAsync<LiveGameStatusDTO>())!;
        return content;
    }

    public async Task StartQuiz(string gameId)
    {
        var res = await httpClient.PostAsJsonAsync($"/live/start", new LiveBasicHostRequestDTO
        {
            GameId = gameId
        });

        if (!res.IsSuccessStatusCode)
            throw new HttpRequestException($"Http status code {(int)res.StatusCode}: {res.ReasonPhrase}");
    }

    public async Task ContinueQuiz(string gameId)
    {
        var res = await httpClient.PostAsJsonAsync($"/live/continue", new LiveBasicHostRequestDTO
        {
            GameId = gameId
        });

        if (!res.IsSuccessStatusCode)
            throw new HttpRequestException($"Http status code {(int)res.StatusCode}: {res.ReasonPhrase}");
    }
}