using ApiContracts;
using GrpcClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizDTO = ApiContracts.QuizDTO;

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
    public ActionResult<string> New([FromBody] LiveCreateQuizRequestDTO quiz)
    {
        int userId = int.Parse(User.FindFirst("Id")!.Value);

        LiveGame game = gameService.CreateGame(quiz.QuizId, userId);

        return Ok(new LiveCreateQuizResponseDTO
        {
            GameId = game.GameId
        });
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
    [HttpGet("status")]
    public async Task<ActionResult<LiveGameStatusDTO>> Status([FromQuery] string gameId, [FromQuery] bool force = false)
    {
        LiveGame? game = gameService.GetGame(gameId);
        
        if (game is null) return BadRequest("Game not found");

        LiveGame state = await game.GetGameState(force);

        return new LiveGameStatusDTO
        {
            Quiz = new QuizDTO
            {
                Title = state.Quiz.Title,
                Visibility = state.Quiz.Visibility,
                CreatorId = state.Quiz.CreatorId,
                Id = state.Quiz.Id
            },
            JoinCode = state.JoinCode,
            CurrentState = state.CurrentState,
            CurrentQuestionId = state.CurrentQuestionId,
            QuizId = state.QuizId,
            GameId = state.GameId,
            Questions = state.Questions.Select(question => new LiveGameQuestionDTO
            {
                Title = question.Title,
                QuestionId = question.QuestionId,
                Answers = question.Answers.Select(answer => new LiveGameAnswerDTO
                {
                    Title = answer.Title,
                    AnswerId = answer.AnswerId,
                    IsCorrect = answer.IsCorrect
                }).ToList()
            }).ToList()
        };
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