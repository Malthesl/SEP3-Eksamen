using System.Text.Json;
using ApiContracts;

namespace BlazorApp.Components.Services;

public class HttpQuestionService : IQuestionService
{
    private HttpClient _client;

    public HttpQuestionService(HttpClient client)
    {
        _client = client;
    }
    
    public async Task<List<QuestionDTO>> GetAllQuestionsAsync()
    {
        List<QuestionDTO>? questions = await _client.GetFromJsonAsync<List<QuestionDTO>>("Questions");
        if (questions is null) throw new NullReferenceException();

        return questions;
    }

    public async Task<QuestionDTO> GetQuestionByIdAsync(int id)
    {
        QuestionDTO? question = await _client.GetFromJsonAsync<QuestionDTO>($"Questions/{id}");
        if (question is null) throw new NullReferenceException();
        
        return question;
    }

    public async Task<QuestionDTO> AddQuestionAsync(CreateQuestionDTO question)
    {
        HttpResponseMessage httpResponse = await _client.PostAsJsonAsync("Questions", question);
        string response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode) throw new Exception(response);
        
        return JsonSerializer.Deserialize<QuestionDTO>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }
}