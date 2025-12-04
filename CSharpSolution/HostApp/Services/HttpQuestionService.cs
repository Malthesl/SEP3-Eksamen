using ApiContracts;

namespace HostApp.Services;

public class HttpQuestionService(HttpClient httpClient) : IQuestionService
{
    public async Task<List<QuestionDTO>> GetQuestionsFromQuiz(int quizId)
    {
        var res = await httpClient.GetAsync($"questions/?quizId={quizId}");
        if (!res.IsSuccessStatusCode) throw new HttpRequestException($"Http status code {(int)res.StatusCode}: {res.ReasonPhrase}");
        
        var content = await res.Content.ReadFromJsonAsync<List<QuestionDTO>>();
        if (content == null) throw new NullReferenceException($"Response is null");
        return content;
    }

    public async Task<QuestionDTO> AddQuestion(CreateQuestionDTO question)
    {
        var res = await httpClient.PostAsJsonAsync($"questions", question);
        if (!res.IsSuccessStatusCode) throw new HttpRequestException($"Http status code {(int)res.StatusCode}: {res.ReasonPhrase}");
        
        var content = await res.Content.ReadFromJsonAsync<QuestionDTO>();
        if (content == null) throw new NullReferenceException($"Response is null");
        return content;
    }

    public async Task UpdateQuestion(QuestionDTO question)
    {
        var res = await httpClient.PostAsJsonAsync($"questions/{question.QuestionId}", question);
        if (!res.IsSuccessStatusCode) throw new HttpRequestException($"Http status code {(int)res.StatusCode}: {res.ReasonPhrase}");
    }

    public async Task DeleteQuestion(int questionId)
    {
        var res = await httpClient.DeleteAsync($"questions/{questionId}");
        if (!res.IsSuccessStatusCode) throw new HttpRequestException($"Http status code {(int)res.StatusCode}: {res.ReasonPhrase}");
    }

    public async Task<QuestionDTO> GetQuestion(int questionId)
    {
        var res = await httpClient.GetAsync($"questions/{questionId}");
        if (!res.IsSuccessStatusCode) throw new HttpRequestException($"Http status code {(int)res.StatusCode}: {res.ReasonPhrase}");
        
        var content = await res.Content.ReadFromJsonAsync<QuestionDTO>();
        if (content == null) throw new NullReferenceException($"Response is null");
        return content;
    }
}