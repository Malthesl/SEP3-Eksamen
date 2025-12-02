using ApiContracts;

namespace BlazorApp.Services;

public interface IQuizService
{
    Task<QuizQueryDTO> QueryMany(string? query, List<string>? visibility, int? creatorId, int start = 0, int count = 20);
}