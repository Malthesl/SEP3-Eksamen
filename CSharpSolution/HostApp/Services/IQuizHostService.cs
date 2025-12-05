using ApiContracts;

namespace HostApp.Services;

public interface IQuizHostService
{
    Task<string> HostQuiz(int quizId);
    
    Task<LiveGameHostStatusDTO> GetGameInfo(string gameId, bool force = false);
    
    Task StartQuiz(string gameId);
    
    Task ContinueQuiz(string gameId);
}