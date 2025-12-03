using ApiContracts;

namespace BlazorApp.Services;

public class HttpAnswerService(HttpClient httpClient) : IAnswerService
{
    public async Task<List<AnswerDTO>> GetAnswersFromQuestionAsync(int questionId)
    {
        var res = await httpClient.GetAsync($"Answers/FromQuestion/{questionId}");
        if (!res.IsSuccessStatusCode) throw new HttpRequestException($"Http status code {(int)res.StatusCode}: {res.ReasonPhrase}");
        
        var content = await res.Content.ReadFromJsonAsync<List<AnswerDTO>>();
        if (content == null) throw new NullReferenceException($"Response is null");
        return content;
    }

    public Task<AnswerDTO> AddAnswerAsync(CreateAnswerDTO answer)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAnswerAsync(AnswerDTO answer)
    {
        var res = await httpClient.PutAsJsonAsync($"Answers/{answer.AnswerId}", answer);
        if (!res.IsSuccessStatusCode) throw new HttpRequestException($"Http status code {(int)res.StatusCode}: {res.ReasonPhrase}");
    }

    public Task DeleteAnswerAsync(int answerId)
    {
        throw new NotImplementedException();
    }
}