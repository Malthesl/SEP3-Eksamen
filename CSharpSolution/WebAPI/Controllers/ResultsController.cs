using ApiContracts;
using GrpcClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AnswerDTO = ApiContracts.AnswerDTO;
using QuestionDTO = ApiContracts.QuestionDTO;
using QuizDTO = ApiContracts.QuizDTO;

namespace WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ResultsController(
    QuizService.QuizServiceClient quizService,
    QuestionService.QuestionServiceClient questionService,
    AnswerService.AnswerServiceClient answerService,
    ResultService.ResultServiceClient resultService,
    AuthorizationService authService)
    : ControllerBase
{
    [HttpGet("{gameId}")]
    public async Task<ActionResult<GameResultDTO>> GetGame([FromRoute] string gameId)
    {
        var res = await resultService.GetGameAsync(new GetGameRequest { GameId = gameId });

        if (!await authService.IsAuthorizedToAccessGame(gameId, User)) return Unauthorized();

        var quiz = await quizService.GetQuizAsync(new GetQuizRequest { QuizId = res.Game.QuizId });

        return Ok(new GameResultDTO
        {
            GameId = res.Game.Id,
            HostUserId = res.Game.HostId,
            PlayedTime = res.Game.PlayedTime,
            QuizId = res.Game.QuizId,
            Quiz = new QuizDTO
            {
                Id = quiz.Quiz.Id,
                Title = quiz.Quiz.Title,
                Visibility = quiz.Quiz.Visibility,
                CreatorId = quiz.Quiz.CreatorId
            }
        });
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GameResultDTO>>> GetGames()
    {
        if (authService.GetUserId(User) is not { } userId) return Unauthorized();

        var res = await resultService.GetGamesHostedByUserAsync(new GetGamesHostedByUserRequest { UserId = userId });

        return Ok(res.Games.Select(g =>
        {
            var quiz = quizService.GetQuiz(new GetQuizRequest { QuizId = g.QuizId });

            return new GameResultDTO
            {
                GameId = g.Id,
                HostUserId = g.HostId,
                PlayedTime = g.PlayedTime,
                QuizId = g.QuizId,
                Quiz = new QuizDTO
                {
                    Id = quiz.Quiz.Id,
                    Title = quiz.Quiz.Title,
                    Visibility = quiz.Quiz.Visibility,
                    CreatorId = quiz.Quiz.CreatorId
                }
            };
        }));
    }

    [HttpGet("{gameId}/participants")]
    public async Task<ActionResult<IEnumerable<GameParticipantDTO>>> GetParticipantsInGame([FromRoute] string gameId)
    {
        if (!await authService.IsAuthorizedToAccessGame(gameId, User)) return Unauthorized();

        var participants =
            await resultService.GetParticipantsInGameAsync(new GetParticipantsInGameRequest { GameId = gameId });

        var results = await resultService.GetGameResultsAsync(new GetGameResultsRequest { GameId = gameId });
        
        return Ok(participants.Participants.Select(p =>
        {
            return new GameParticipantDTO
            {
                GameId = p.GameId,
                ParticipantId = p.Id,
                ParticipantName = p.Name,
                Score = p.Score,
                Answers = results.Answers
                    .Where(a => a.ParticipantId == p.Id)
                    .Select(a =>
                    {
                        var answer = answerService.GetAnswer(new GetAnswerRequest { Id = a.Answer }).Answer;
                        var answers = answerService.GetAllAnswersInQuestion(new GetAllAnswersInQuestionRequest
                            { QuestionId = answer.QuestionId }).Answers;

                        return new GameParticipantAnswerDTO
                        {
                            GameId = p.GameId,
                            AnswerId = a.Answer,
                            ParticipantId = p.Id,
                            ParticipantName = p.Name,
                            Answers = answers.Select(qa => new AnswerDTO
                            {
                                AnswerId = qa.Id,
                                QuestionId = qa.QuestionId,
                                Index = qa.Index,
                                IsCorrect = qa.IsCorrect,
                                Title = qa.Title
                            })
                        };
                    })
            };
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

            var answer = answerService.GetAnswer(new GetAnswerRequest { Id = a.Answer }).Answer;
            var answers = answerService.GetAllAnswersInQuestion(new GetAllAnswersInQuestionRequest
                { QuestionId = answer.QuestionId }).Answers;

            return new GameParticipantAnswerDTO
            {
                GameId = p.GameId,
                AnswerId = a.Answer,
                ParticipantId = p.Id,
                ParticipantName = p.Name,
                Answers = answers.Select(qa => new AnswerDTO
                {
                    AnswerId = qa.Id,
                    QuestionId = qa.QuestionId,
                    Index = qa.Index,
                    IsCorrect = qa.IsCorrect,
                    Title = qa.Title
                })
            };
        }));
    }
}