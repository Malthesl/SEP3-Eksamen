using ApiContracts;
using GrpcClient;
using Microsoft.AspNetCore.Mvc;
using QuizDTO = ApiContracts.QuizDTO;
using UserDTO = ApiContracts.UserDTO;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class QuizzesController(QuizService.QuizServiceClient quizService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<QuizDTO>> CreateQuiz([FromBody] CreateQuizDTO quizDto)
    {
        int userId = int.Parse(User.FindFirst("Id")!.Value);

        var res = await quizService.AddQuizAsync(new AddQuizRequest
        {
            Title = quizDto.Title,
            CreatorId = userId
        });

        return Created($"Quiz/{res.QuizDto.Id}", new QuizDTO()
        {
            Id = res.QuizDto.Id,
            Title = res.QuizDto.Title,
            Visibility = res.QuizDto.Visibility,
            CreatorId = res.QuizDto.CreatorId
        });
    }

    [HttpPost("{quizId:int}")]
    public async Task<ActionResult<QuizDTO>> UpdateQuiz([FromRoute] int quizId, [FromBody] UpdateQuizDTO quizDto)
    {
        await quizService.UpdateQuizAsync(new UpdateQuizRequest
        {
            Quiz = new GrpcClient.QuizDTO
            {
                Title = quizDto.Title,
                Visibility = quizDto.Visibility,
                Id = quizId
            }
        });

        return await GetQuiz(quizId);
    }

    [HttpDelete("{quizId:int}")]
    public async Task<ActionResult> DeleteQuiz([FromRoute] int quizId)
    {
        await quizService.DeleteQuizAsync(new DeleteQuizRequest() { QuizId = quizId });
        return Ok();
    }

    [HttpGet("{quizId:int}")]
    public async Task<ActionResult<QuizDTO>> GetQuiz([FromRoute] int quizId)
    {
        var res = await quizService.GetQuizAsync(new GetQuizRequest()
        {
            QuizId = quizId
        });
        return Ok(new QuizDTO
        {
            Id = res.Quiz.Id,
            Title = res.Quiz.Title,
            Visibility = res.Quiz.Visibility,
            CreatorId = res.Quiz.CreatorId,
            Creator = new UserDTO
            {
                Id = res.Quiz.CreatorId,
                Username = res.Quiz.Creator.Username
            },
            QuestionCount = res.Quiz.QuestionCount
        });
    }

    /**
     * Querier alle quizzes fra backend
     */
    [HttpGet]
    public ActionResult<QuizQueryDTO> QueryMany([FromQuery] string? query, [FromQuery] string? visibility,
        [FromQuery] int? creatorId, [FromQuery] int start = 0, [FromQuery] int count = 20)
    {
        var req = new QueryQuizzesRequest
        {
            SearchQuery = query ?? "",
            ByCreatorId = creatorId ?? -1,
            Start = start,
            End = start + count
        };

        req.Visibilities.AddRange(visibility?.Split(',') ?? []);

        var res = quizService.QueryQuizzes(req);

        return Ok(new QuizQueryDTO
        {
            Start = res.Start,
            End = res.End,
            Count = res.Count,
            Quizzes = res.Quizzes.Select(quiz => new QuizDTO
            {
                Id = quiz.Id,
                Title = quiz.Title,
                CreatorId = quiz.CreatorId,
                Creator = new UserDTO
                {
                    Id = quiz.CreatorId,
                    Username = quiz.Creator.Username
                },
                Visibility = quiz.Visibility,
                QuestionCount = quiz.QuestionCount
            })
        });
    }
}