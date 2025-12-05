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
    public ActionResult Start([FromBody] LiveBasicHostRequestDTO request)
    {
        int userId = int.Parse(User.FindFirst("Id")!.Value);

        LiveGame game = gameService.GetGame(request.GameId);

        if (game is null) return NotFound("Game not found");
        if (game.HostUserId != userId) return Unauthorized("Kun host kan starte quizzen");

        game.Start();

        return Ok();
    }

    [Authorize]
    [HttpPost("continue")]
    public ActionResult Continue([FromBody] LiveBasicHostRequestDTO request)
    {
        int userId = int.Parse(User.FindFirst("Id")!.Value);

        LiveGame game = gameService.GetGame(request.GameId);

        if (game is null) return NotFound("Game not found");
        if (game.HostUserId != userId) return Unauthorized("Kun host kan starte quizzen");

        game.Continue();

        return Ok();
    }

    [Authorize]
    [HttpPost("kick")]
    public ActionResult Kick()
    {
        throw new NotImplementedException();
    }

    // Delt
    [HttpGet("status")]
    public async Task<ActionResult<dynamic>> Status(
        [FromQuery] string gameId,
        [FromQuery] string? playerId,
        [FromQuery] int lastUpdateNo = 0)
    {
        LiveGame? game = gameService.GetGame(gameId);

        if (game is null) return BadRequest("Game not found");

        LiveGame state = await game.GetGameState(lastUpdateNo);

        string? userIdString = User.FindFirst("Id")?.Value;

        // HOST STATUS
        if (userIdString is not null)
        {
            if (int.Parse(userIdString) != game.HostUserId) return Unauthorized();

            return new LiveGameHostStatusDTO
            {
                UpdateNo = lastUpdateNo,
                RelTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                CountdownToTime = state.NextEventTime,
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
                HostUserId = state.HostUserId,
                PlayersAnswered = state.Players.Count(player => player.LatestAnswerId != null),
                Questions = state.Questions.Select(question => new LiveGameQuestionDTO
                {
                    Title = question.Title,
                    QuestionId = question.QuestionId,
                    Answers = question.Answers.Select(answer => new LiveGameAnswerDTO
                    {
                        Title = answer.Title,
                        AnswerId = answer.AnswerId,
                        IsCorrect = answer.IsCorrect,
                        Index = answer.Index
                    }).ToList()
                }).ToList(),
                Players = state.Players.Select(player => new LiveGamePlayerDTO
                {
                    PlayerId = player.PlayerId,
                    Name = player.Name,
                    Score = player.Score,
                    LatestAnswerId = player.LatestAnswerId,
                    LatestScoreChange = player.LatestScoreChange,
                    LatestAnswerCorrect = state.CurrentQuestion?.Answers.Find(a => a.AnswerId == player.LatestAnswerId)
                        ?.IsCorrect
                }).ToList()
            };
        }
        // PLAYER STATUS
        if (playerId is not null && game.Players.Any(player => player.PlayerId == playerId))
        {
            LiveGamePlayer player = game.Players.Find(player => player.PlayerId == playerId)!;

            return new LiveGamePlayerStatusDTO
            {
                UpdateNo = lastUpdateNo,
                RelTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                CountdownToTime = state.NextEventTime,
                CurrentState = state.CurrentState,
                CurrentQuestionId = state.CurrentQuestionId,
                Questions = state.Questions.Select(q => new LiveGameQuestionCensoredDTO
                {
                    QuestionId = q.QuestionId,
                    Title = q.Title,
                    Answers = q.Answers.Select(a => new LiveGameAnswerCensoredDTO
                    {
                        AnswerId = a.AnswerId,
                        Title = a.Title,
                        Index = a.Index
                    }).ToList()
                }).ToList(),
                Score = player.Score,
                GameId = state.GameId,
                PlayerId = player.PlayerId,
                HostUserId = state.HostUserId,
                Name = player.Name,
                QuizId = state.QuizId,
                LatestAnswerId = player.LatestAnswerId,
                LatestScoreChange = player.LatestScoreChange,
                LatestAnswerCorrect = state.CurrentQuestion?.Answers.Find(a => a.AnswerId == player.LatestAnswerId)
                    ?.IsCorrect
            };
        }

        return BadRequest("Mangler host login eller et player id");
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
        [FromBody] LiveGameAnswerRequestDTO request
    )
    {
        LiveGame? game = gameService.GetGame(request.GameId);

        if (game is null) return NotFound("Spillet findes ikke.");

        game.Answer(request.QuestionId, request.AnswerId, request.PlayerId);

        return Ok();
    }
}