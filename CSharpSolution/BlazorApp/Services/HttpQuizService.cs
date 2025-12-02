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

        var url = QueryHelpers.AddQueryString("quiz", queryParams);

        var response = await httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode) throw new Exception("Fejl i søgning af quizzer. " + response.StatusCode);

        return (await response.Content.ReadFromJsonAsync<QuizQueryDTO>())!;
    }
}