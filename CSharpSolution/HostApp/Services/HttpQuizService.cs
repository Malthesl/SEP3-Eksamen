using ApiContracts;
using Microsoft.AspNetCore.WebUtilities;
using static HostApp.Utils.Utils;

namespace HostApp.Services;

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
        await CheckResponse(response);
        
        return (await response.Content.ReadFromJsonAsync<QuizQueryDTO>())!;
    }

    public async Task<QuizDTO> CreateQuiz(string title)
    {
        var response = await httpClient.PostAsJsonAsync("quizzes", new { Title = title });
        await CheckResponse(response);
        
        return (await response.Content.ReadFromJsonAsync<QuizDTO>())!;
    }

    public async Task<QuizDTO> UpdateQuiz(int id, string newTitle, string newVisibility)
    {
        var response = await httpClient.PostAsJsonAsync($"quizzes/{id}", new UpdateQuizDTO { Title = newTitle, Visibility = newVisibility });
        await CheckResponse(response);
        
        return (await response.Content.ReadFromJsonAsync<QuizDTO>())!;
    }

    public async Task DeleteQuiz(int id)
    {
        var response = await httpClient.DeleteAsync($"quizzes/{id}");
        await CheckResponse(response);
    }

    public async Task<QuizDTO> GetQuiz(int id)
    {
        var response = await httpClient.GetAsync($"quizzes/{id}");
        await CheckResponse(response);
        
        return (await response.Content.ReadFromJsonAsync<QuizDTO>())!;
    }

}