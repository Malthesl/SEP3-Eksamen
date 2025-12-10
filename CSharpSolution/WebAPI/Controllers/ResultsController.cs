using ApiContracts;
using GrpcClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ResultsController(ResultService.ResultServiceClient resultService, AuthorizationService authService)
    : ControllerBase
{
    [HttpGet("{gameId}")]
    public async Task<ActionResult<GameResultDTO>> GetGame([FromRoute] string gameId)
    {
        var res = await resultService.GetGameAsync(new GetGameRequest { GameId = gameId });

        if (!await authService.IsAuthorizedToAccessGame(gameId, User)) return Unauthorized();

        return Ok(new GameResultDTO
        {
            GameId = res.Game.Id,
            HostUserId = res.Game.HostId,
            PlayedTime = res.Game.PlayedTime,
            QuizId = res.Game.QuizId
        });
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GameResultDTO>>> GetGames()
    {
        if (authService.GetUserId(User) is not { } userId) return Unauthorized();

        var res = await resultService.GetGamesHostedByUserAsync(new GetGamesHostedByUserRequest { UserId = userId });

        return Ok(res.Games.Select(g => new GameResultDTO
        {
            GameId = g.Id,
            HostUserId = g.HostId,
            PlayedTime = g.PlayedTime,
            QuizId = g.QuizId
        }));
    }

    [HttpGet("{gameId}/participants")]
    public async Task<ActionResult<IEnumerable<GameParticipantDTO>>> GetParticipantsInGame([FromRoute] string gameId)
    {
        if (!await authService.IsAuthorizedToAccessGame(gameId, User)) return Unauthorized();

        var participants =
            await resultService.GetParticipantsInGameAsync(new GetParticipantsInGameRequest { GameId = gameId });

        var results = await resultService.GetGameResultsAsync(new GetGameResultsRequest { GameId = gameId });

        return Ok(participants.Participants.Select(p => new GameParticipantDTO
        {
            GameId = p.GameId,
            ParticipantId = p.Id,
            ParticipantName = p.Name,
            Answers = results.Answers
                .Where(a => a.ParticipantId == p.Id)
                .Select(a => new GameParticipantAnswerDTO
                {
                    GameId = p.GameId,
                    AnswerId = a.Answer,
                    ParticipantId = p.Id,
                    ParticipantName = p.Name,
                })
        }));
    }

    [HttpGet("{gameId}/questions/{questionId:int}/answers")]
    public async Task<ActionResult<IEnumerable<GameParticipantDTO>>> GetResultsForQuestion([FromRoute] string gameId,
        [FromRoute] int questionId)
    {
        if (!await authService.IsAuthorizedToAccessGame(gameId, User)) return Unauthorized();

        var participants =
            await resultService.GetParticipantsInGameAsync(new GetParticipantsInGameRequest { GameId = gameId });

        var results = await resultService.GetResultOnQuestionAsync(new GetResultOnQuestionRequest
            { GameId = gameId, QuestionId = questionId });

        return Ok(results.Answers.Select(a =>
        {
            var p = participants.Participants.Single(p => p.Id == a.ParticipantId);

            return new GameParticipantAnswerDTO
            {
                GameId = p.GameId,
                AnswerId = a.Answer,
                ParticipantId = p.Id,
                ParticipantName = p.Name,
            };
        }));
    }
}