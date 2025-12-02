using System.Security.Claims;
using ApiContracts;
using GrpcClient;
using Microsoft.AspNetCore.Mvc;
using QuizDTO = ApiContracts.QuizDTO;
using UserDTO = ApiContracts.UserDTO;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class QuizController(QuizzesService.QuizzesServiceClient quizService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<QuizDTO>> CreateQuiz([FromBody] CreateQuizDTO quizDto)
    {
        try
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
                CreatorId = res.QuizDto.CreatorId,
                Creator = new UserDTO
                {
                    Id = res.QuizDto.CreatorId,
                    Username = res.QuizDto.Creator.Username
                },
                QuestionCount = res.QuizDto.QuestionCount
            });
        } 
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("{quizId:int}")]
    public async Task<ActionResult<QuizDTO>> UpdateQuiz([FromBody] QuizDTO quizDto)
    {
        try
        {
            await quizService.UpdateQuizAsync(new UpdateQuizRequest()
            {
                Quiz = new GrpcClient.QuizDTO()
                {
                    Title = quizDto.Title,
                    Visibility = quizDto.Visibility,
                    Id = quizDto.Id
                }
            });
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{quizId:int}")]
    public async Task<ActionResult> DeleteQuiz([FromRoute] int quizId)
    {
        try
        {
            await quizService.DeleteQuizAsync(new DeleteQuizRequest() { QuizId = quizId });
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{quizId:int}")]
    public async Task<ActionResult<QuizDTO>> GetQuiz([FromRoute] int quizId)
    {
        try
        {
            var res = await quizService.GetQuizAsync(new GetQuizRequest()
            {
                QuizId = quizId
            });
            return Ok(new QuizDTO()
            {
                Id = res.Quiz.Id,
                Title = res.Quiz.Title,
                Visibility = res.Quiz.Visibility,
                CreatorId = res.Quiz.CreatorId,
                Creator = new UserDTO
                {
                    Id = res.Quiz.CreatorId,
                    Username = res.Quiz.Creator.Username
                }
            });
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
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