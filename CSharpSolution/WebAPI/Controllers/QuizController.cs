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