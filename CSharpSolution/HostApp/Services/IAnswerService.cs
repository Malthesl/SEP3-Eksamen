using ApiContracts;

namespace BlazorApp.Services;

public interface IAnswerService
{
    Task<List<AnswerDTO>> GetAnswersFromQuestionAsync(int questionId);
    Task<AnswerDTO> AddAnswerAsync(CreateAnswerDTO answer);
    Task UpdateAnswerAsync(AnswerDTO answer);
    Task DeleteAnswerAsync(int answerId);
}