using ApiContracts;

namespace HostApp.Services;

public interface IAnswerService
{
    Task<List<AnswerDTO>> GetAnswersFromQuestionAsync(int questionId);
    Task<AnswerDTO> AddAnswerAsync(CreateAnswerDTO answer);
    Task UpdateAnswerAsync(AnswerDTO answer);
    Task DeleteAnswerAsync(int answerId);
}