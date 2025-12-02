using ApiContracts;

namespace BlazorApp.Services;

public class HttpQuizService(HttpClient httpClient) : IQuizService
{
    public async Task<QuizQueryDTO> QueryMany(string? query, List<string>? visibility, int? creatorId, int start = 0, int count = 20)
    {
        var response = await httpClient.GetAsync("quiz");
        if (!response.IsSuccessStatusCode) throw new Exception("Fejl i søgning af quizzer. " + response.StatusCode);

        return (await response.Content.ReadFromJsonAsync<QuizQueryDTO>())!;
    }
}