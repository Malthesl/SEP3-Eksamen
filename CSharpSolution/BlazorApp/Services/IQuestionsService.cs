using ApiContracts;

namespace BlazorApp.Services;

public interface IQuestionsService
{
    Task<List<QuestionDTO>> GetQuestionsFromQuiz(int quizId);
    Task<QuestionDTO> AddQuestion(QuestionDTO question);
    Task UpdateQuestion(QuestionDTO question);
    Task DeleteQuestion(int questionId);
    Task<QuestionDTO> GetQuestion(int questionId);
}