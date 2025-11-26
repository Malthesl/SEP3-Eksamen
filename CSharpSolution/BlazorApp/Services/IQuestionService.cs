using ApiContracts;

namespace BlazorApp.Services;

public interface IQuestionService
{
    Task<QuestionListDTO> GetAllQuestionsAsync();
    Task<QuestionDTO> GetQuestionByIdAsync(int id);
    Task<QuestionDTO> AddQuestionAsync(CreateQuestionDTO question);
}