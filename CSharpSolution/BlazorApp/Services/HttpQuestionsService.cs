using ApiContracts;

namespace BlazorApp.Services;

public class HttpQuestionsService(HttpClient httpClient) : IQuestionsService
{
    public async Task<List<QuestionDTO>> GetQuestionsFromQuiz(int quizId)
    {
        var res = await httpClient.GetAsync($"Question/FromQuiz/{quizId}");
        if (!res.IsSuccessStatusCode) throw new HttpRequestException($"Http status code {(int)res.StatusCode}: {res.ReasonPhrase}");
        
        var content = await res.Content.ReadFromJsonAsync<List<QuestionDTO>>();
        if (content == null) throw new NullReferenceException($"Response is null");
        return content;
    }

    public async Task<QuestionDTO> AddQuestion(CreateQuestionDTO question)
    {
        var res = await httpClient.PostAsJsonAsync($"Question", question);
        if (!res.IsSuccessStatusCode) throw new HttpRequestException($"Http status code {(int)res.StatusCode}: {res.ReasonPhrase}");
        
        var content = await res.Content.ReadFromJsonAsync<QuestionDTO>();
        if (content == null) throw new NullReferenceException($"Response is null");
        return content;
    }

    public async Task UpdateQuestion(QuestionDTO question)
    {
        var res = await httpClient.PostAsJsonAsync($"Question/{question.QuestionId}", question);
        if (!res.IsSuccessStatusCode) throw new HttpRequestException($"Http status code {(int)res.StatusCode}: {res.ReasonPhrase}");
    }

    public async Task DeleteQuestion(int questionId)
    {
        var res = await httpClient.DeleteAsync($"Question/{questionId}");
        if (!res.IsSuccessStatusCode) throw new HttpRequestException($"Http status code {(int)res.StatusCode}: {res.ReasonPhrase}");
    }

    public async Task<QuestionDTO> GetQuestion(int questionId)
    {
        var res = await httpClient.GetAsync($"Question/{questionId}");
        if (!res.IsSuccessStatusCode) throw new HttpRequestException($"Http status code {(int)res.StatusCode}: {res.ReasonPhrase}");
        
        var content = await res.Content.ReadFromJsonAsync<QuestionDTO>();
        if (content == null) throw new NullReferenceException($"Response is null");
        return content;
    }
}