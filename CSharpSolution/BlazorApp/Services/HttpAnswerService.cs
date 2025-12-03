using ApiContracts;

namespace BlazorApp.Services;

public class HttpAnswerService(HttpClient httpClient) : IAnswerService
{
    public async Task<List<AnswerDTO>> GetAnswersFromQuestionAsync(int questionId)
    {
        var res = await httpClient.GetAsync($"answers?questionId={questionId}");
        if (!res.IsSuccessStatusCode) throw new HttpRequestException($"Http status code {(int)res.StatusCode}: {res.ReasonPhrase}");
        
        var content = await res.Content.ReadFromJsonAsync<List<AnswerDTO>>();
        if (content == null) throw new NullReferenceException($"Response is null");
        return content;
    }

    public async Task<AnswerDTO> AddAnswerAsync(CreateAnswerDTO answer)
    {
        var res = await httpClient.PostAsJsonAsync($"answers", answer);
        if (!res.IsSuccessStatusCode) throw new HttpRequestException($"Http status code {(int)res.StatusCode}: {res.ReasonPhrase}");
        
        var content = await res.Content.ReadFromJsonAsync<AnswerDTO>();
        if (content == null) throw new NullReferenceException($"Response is null");
        return content;
    }

    public async Task UpdateAnswerAsync(AnswerDTO answer)
    {
        var res = await httpClient.PostAsJsonAsync($"answers/{answer.AnswerId}", answer);
        if (!res.IsSuccessStatusCode) throw new HttpRequestException($"Http status code {(int)res.StatusCode}: {res.StatusCode}");
    }

    public async Task DeleteAnswerAsync(int answerId)
    {
        var res = await httpClient.DeleteAsync($"answers/{answerId}");
        if (!res.IsSuccessStatusCode) throw new HttpRequestException($"Http status code {(int)res.StatusCode}");
    }
}