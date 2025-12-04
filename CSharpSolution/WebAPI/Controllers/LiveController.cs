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
    public ActionResult Start()
    {
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpPost("continue")]
    public ActionResult Continue()
    {
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpPost("kick")]
    public ActionResult Kick()
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
            }).ToList(),
            Players = state.Players.Select(player => new LiveGamePlayerDTO
            {
                PlayerId = player.PlayerId,
                Name = player.Name
            }).ToList()
        };
    }

    // Deltagere
    [HttpPost("join")]
    public ActionResult<LiveGameJoinResponseDTO> Join([FromBody] LiveGameJoinRequestDTO request)
    {
        LiveGame? game = gameService.GetGameFromJoinCode(request.JoinCode);
        
        if (game is null) return NotFound($"Spillet med join koden {request.JoinCode} var ikke fundet.");

        LiveGamePlayer player = game.JoinGame(request.Name);
        
        return Ok(new LiveGameJoinResponseDTO
        {
            PlayerId = player.PlayerId,
            GameId = game.GameId
        });
    }

    [HttpPost("answer")]
    public ActionResult Answer(
        // [FromQuery] string gameId,
        // [FromQuery] string playerId,
        // [FromQuery] int questionId,
        // [FromQuery] int answerId    ... Skal være en DTO fra FromBody, føler det er mere rest
    )
    {
        throw new NotImplementedException();
    }
}