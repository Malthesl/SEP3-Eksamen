using GrpcClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class LiveController(
    QuizService.QuizServiceClient quizService,
    QuestionService.QuestionServiceClient questionService,
    AnswerService.AnswerServiceClient answerService,
    LiveGameService gameService) : ControllerBase
{
    // Host
    [Authorize]
    [HttpPost("new")]
    public ActionResult<string> New([FromQuery] int quizId)
    {
        int userId = int.Parse(User.FindFirst("Id")!.Value);

        LiveGame game = gameService.CreateGame(quizId, userId);

        return Ok(game.GameId);
    }

    [Authorize]
    [HttpPost("start")]
    public ActionResult Start([FromQuery] string gameId)
    {
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpPost("continue")]
    public ActionResult Continue([FromQuery] string gameId)
    {
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpPost("kick")]
    public ActionResult Kick([FromQuery] string gameId, [FromQuery] string playerId)
    {
        throw new NotImplementedException();
    }

    // Delt
    [HttpPost("status")]
    public async Task<ActionResult<LiveGame>> Status([FromQuery] string gameId, [FromQuery] bool force = false)
    {
        LiveGame? game = gameService.GetGame(gameId);
        
        if (game is null) return BadRequest("Game not found");

        return await game.GetGameState(force);
    }

    // Deltagere
    [HttpPost("join")]
    public ActionResult Join([FromQuery] string code)
    {
        throw new NotImplementedException();
    }

    [HttpPost("answer")]
    public ActionResult Answer(
        [FromQuery] string gameId,
        [FromQuery] string playerId,
        [FromQuery] int questionId,
        [FromQuery] int answerId)
    {
        throw new NotImplementedException();
    }
}