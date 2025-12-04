using ApiContracts;

namespace BlazorApp.Services;

public interface IQuestionService
{
    Task<List<QuestionDTO>> GetQuestionsFromQuiz(int quizId);
    Task<QuestionDTO> AddQuestion(CreateQuestionDTO question);
    Task UpdateQuestion(QuestionDTO question);
    Task DeleteQuestion(int questionId);
    Task<QuestionDTO> GetQuestion(int questionId);
}