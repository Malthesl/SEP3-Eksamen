using System.Security.Claims;
using ApiContracts;
using QuestionDTO = ApiContracts.QuestionDTO;
using GrpcClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class QuestionController(
    QuestionService.QuestionServiceClient questionService,
    QuizzesService.QuizzesServiceClient quizService) : ControllerBase
{
    [HttpGet("FromQuiz/{quizId:int}")]
    public async Task<ActionResult<List<QuestionDTO>>> GetAllQuestionsInQuiz([FromRoute] int quizId)
    {
        try
        {
            var questionsDto = await questionService.GetAllQuestionsInQuizAsync(new GetAllQuestionsInQuizRequest()
            {
                QuizId = quizId
            });

            List<QuestionDTO> returnList = new List<QuestionDTO>();

            foreach (var question in questionsDto.Questions)
            {
                returnList.Add(new QuestionDTO()
                {
                    QuestionId = question.Id,
                    Title = question.Title,
                    QuizId = question.QuizId,
                    Index = question.Index,
                });
            }

            return Ok(returnList);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<QuestionDTO>> AddQuestion([FromBody] CreateQuestionDTO questionDto)
    {
        try
        {
            var res = await questionService.AddQuestionAsync(new AddQuestionRequest()
            {
                QuizId = questionDto.QuizId,
                Title = questionDto.Title,
            });

            return Created($"Questions/{res.NewQuestion.Id}", new QuestionDTO()
            {
                Index = res.NewQuestion.Index,
                Title = res.NewQuestion.Title,
                QuestionId = res.NewQuestion.Id,
                QuizId = res.NewQuestion.QuizId,
            });
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize]
    [HttpPost("{questionId:int}")]
    public async Task<ActionResult<QuestionDTO>> UpdateQuestion([FromBody] QuestionDTO questionDto)
    {
        try
        {
            if (!await IsAuthorizedToChange(questionDto.QuizId, User)) return Unauthorized();

            await questionService.UpdateQuestionAsync(new UpdateQuestionRequest()
            {
                UpdatedQuestion = new GrpcClient.QuestionDTO()
                {
                    QuizId = questionDto.QuizId,
                    Index = questionDto.Index,
                    Id = questionDto.QuestionId,
                    Title = questionDto.Title,
                }
            });

            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize]
    [HttpDelete("{questionId:int}")]
    public async Task<ActionResult> DeleteQuestion([FromRoute] int questionId)
    {
        try
        {
            if (!await IsAuthorizedToChange(questionId, User)) return Unauthorized();
            
            await questionService.DeleteQuestionAsync(new DeleteQuestionRequest() { QuestionId = questionId });
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize]
    [HttpGet("{questionId:int}")]
    public async Task<ActionResult<QuestionDTO>> GetQuestion([FromRoute] int questionId)
    {
        try
        {
            var res = await questionService.GetQuestionByIdAsync(new GetQuestionByIdRequest()
                { QuestionId = questionId });

            return Ok(new QuestionDTO()
            {
                Index = res.Question.Index,
                Title = res.Question.Title,
                QuestionId = res.Question.Id,
                QuizId = res.Question.QuizId,
            });
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    private async Task<bool> IsAuthorizedToChange(int questionId, ClaimsPrincipal user)
    {
        var question = await questionService.GetQuestionByIdAsync(new GetQuestionByIdRequest{QuestionId = questionId});
        int userId = int.Parse(user.FindFirst("Id")!.Value);
        int quizCreatorId = (await quizService.GetQuizAsync(new GetQuizRequest()
        {
            QuizId = question.Question.QuizId
        })).Quiz.CreatorId;
        
        return quizCreatorId == userId;
    }

    private async Task<bool> IsAuthorizedToAdd(int quizId, ClaimsPrincipal user)
    {
        int userId = int.Parse(user.FindFirst("Id")!.Value);
        int quizCreatorId = (await quizService.GetQuizAsync(new GetQuizRequest()
        {
            QuizId = quizId
        })).Quiz.CreatorId;
        
        return quizCreatorId == userId;
    }

    private async Task<bool> IsAuthorizedToAccess(int questionId, ClaimsPrincipal user)
    {
        
        var question = await questionService.GetQuestionByIdAsync(new GetQuestionByIdRequest{QuestionId = questionId});
        int userId = int.Parse(user.FindFirst("Id")!.Value);
        var quiz = (await quizService.GetQuizAsync(new GetQuizRequest()
        {
            QuizId = question.Question.QuizId
        })).Quiz;
        
        return quiz.CreatorId == userId || quiz.Visibility == "public";
    }
}