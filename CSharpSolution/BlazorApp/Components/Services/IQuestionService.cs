using ApiContracts;

namespace BlazorApp.Components.Services;

public interface IQuestionService
{
    Task<List<QuestionDTO>> GetAllQuestionsAsync();
    Task<QuestionDTO> GetQuestionByIdAsync(int id);
    Task<QuestionDTO> AddQuestionAsync(CreateQuestionDTO question);
}