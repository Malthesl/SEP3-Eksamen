using ApiContracts;

namespace BlazorApp.Components.Services;

public interface IQuestionService
{
    Task<QuestionListDTO> GetAllQuestionsAsync();
    Task<QuestionDTO> GetQuestionByIdAsync(int id);
    Task<QuestionDTO> AddQuestionAsync(CreateQuestionDTO question);
}