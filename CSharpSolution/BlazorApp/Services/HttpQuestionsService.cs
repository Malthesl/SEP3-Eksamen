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

    public Task<QuestionDTO> AddQuestion(QuestionDTO question)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateQuestion(QuestionDTO question)
    {
        var res = await httpClient.PostAsJsonAsync($"Question/{question.QuestionId}", question);
        if (!res.IsSuccessStatusCode) throw new HttpRequestException($"Http status code {(int)res.StatusCode}: {res.ReasonPhrase}");
    }

    public Task DeleteQuestion(int questionId)
    {
        throw new NotImplementedException();
    }

    public Task<QuestionDTO> GetQuestion(int questionId)
    {
        throw new NotImplementedException();
    }
}