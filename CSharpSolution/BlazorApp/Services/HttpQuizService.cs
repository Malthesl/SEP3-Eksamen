using ApiContracts;
using Microsoft.AspNetCore.WebUtilities;

namespace BlazorApp.Services;

public class HttpQuizService(HttpClient httpClient) : IQuizService
{
    public async Task<QuizQueryDTO> QueryMany(string? query, List<string>? visibility, int? creatorId, int start = 0,
        int count = 20)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["start"] = start.ToString(),
            ["count"] = count.ToString()
        };

        if (query is not null) queryParams.Add("query", query);
        if (visibility is not null) queryParams.Add("visibility", String.Join(",", visibility));
        if (creatorId is not null) queryParams.Add("creatorId", creatorId.ToString());

        var url = QueryHelpers.AddQueryString("quizzes", queryParams);

        var response = await httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode) throw new Exception("Fejl i søgning af quizzer. " + response.StatusCode);

        return (await response.Content.ReadFromJsonAsync<QuizQueryDTO>())!;
    }

    public async Task<QuizDTO> CreateQuiz(string title)
    {
        var response = await httpClient.PostAsJsonAsync("quizzes", new { Title = title });
        if (!response.IsSuccessStatusCode) throw new Exception("Kunne ikke oprette quiz. " + response.StatusCode);

        return (await response.Content.ReadFromJsonAsync<QuizDTO>())!;
    }

    public async Task<QuizDTO> UpdateQuiz(int id, string newTitle, string newVisibility)
    {
        var response = await   httpClient.PostAsJsonAsync($"quizzes/{id}", new { Title = newTitle, Visibility = newVisibility });
        if (!response.IsSuccessStatusCode) throw new Exception("Kunne ikke opdatere quiz. " + response.StatusCode);

        return (await response.Content.ReadFromJsonAsync<QuizDTO>())!;
    }

    public async Task DeleteQuiz(int id)
    {
        var respone = await httpClient.DeleteAsync($"quizzes/{id}");
        if (!respone.IsSuccessStatusCode) throw new Exception("Kunne ikke slette quiz. " + respone.StatusCode);
    }

    public async Task<QuizDTO> GetQuiz(int id)
    {
        var response = await httpClient.GetAsync($"quizzes/{id}");
        if (!response.IsSuccessStatusCode) throw new Exception("Kunne ikke hente quiz. " + response.StatusCode);
        
        return (await response.Content.ReadFromJsonAsync<QuizDTO>())!;
    }

}