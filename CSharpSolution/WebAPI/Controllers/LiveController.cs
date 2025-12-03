using GrpcClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class LiveController(
    QuizzesService.QuizzesServiceClient quizService,
    QuestionService.QuestionServiceClient questionService,
    AnswersService.AnswersServiceClient answerService) : ControllerBase
{
    // Host
    [Authorize]
    [HttpPost("new")]
    public ActionResult<string> New([FromQuery] int quizId)
    {
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpPost("start")]
    public ActionResult<string> Start([FromQuery] string gameId)
    {
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpPost("continue")]
    public ActionResult<string> Continue([FromQuery] string gameId)
    {
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpPost("kick")]
    public ActionResult<string> Kick([FromQuery] string gameId, [FromQuery] int playerId)
    {
        throw new NotImplementedException();
    }

    // Delt
    [HttpPost("status")]
    public ActionResult<string> Status([FromQuery] string gameId)
    {
        throw new NotImplementedException();
    }

    // Deltagere
    [HttpPost("join")]
    public ActionResult<string> Join([FromQuery] string code)
    {
        throw new NotImplementedException();
    }

    [HttpPost("answer")]
    public ActionResult<string> Answer([FromQuery] string gameId, [FromQuery] string playerId,
        [FromQuery] int questionId,
        [FromQuery] int answerId)
    {
        throw new NotImplementedException();
    }
}

public class LiveGame(
    int quizId,
    int userId,
    QuizzesService.QuizzesServiceClient quizService,
    QuestionService.QuestionServiceClient questionService,
    AnswersService.AnswersServiceClient answerService)
{
    public string GameId { get; init; } = Guid.NewGuid().ToString();
    public int HostUserId { get; } = userId;
    public int QuizId { get; } = quizId;

    private readonly List<TaskCompletionSource> _tasks = [];

    public async Task<LiveGame> GetGameState(bool force)
    {
        var t = new TaskCompletionSource();
        _tasks.Add(t);
        await Task.WhenAny(t.Task, Task.Delay(1000 * 30));
        return this;
    }

    public void StateUpdated()
    {
        foreach (var task in _tasks)
        {
            task.SetResult();
        }
        _tasks.Clear();
    }
}