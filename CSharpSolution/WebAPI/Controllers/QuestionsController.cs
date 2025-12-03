using System.Security.Claims;
using ApiContracts;
using QuestionDTO = ApiContracts.QuestionDTO;
using GrpcClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class QuestionsController(
    QuestionService.QuestionServiceClient questionService,
    QuizService.QuizServiceClient quizService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<QuestionDTO>>> GetAllQuestionsInQuiz([FromQuery] int quizId)
    {
        if (!await IsAuthorizedToAccess(quizId, User)) return Unauthorized();

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

    [HttpPost]
    public async Task<ActionResult<QuestionDTO>> AddQuestion([FromBody] CreateQuestionDTO questionDto)
    {
        if (!await IsAuthorizedToAdd(questionDto.QuizId, User)) return Unauthorized();

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

    [HttpPost("{questionId:int}")]
    public async Task<ActionResult<QuestionDTO>> UpdateQuestion([FromRoute] int questionId,
        [FromBody] QuestionDTO questionDto)
    {
        if (!await IsAuthorizedToChange(questionDto.QuestionId, User)) return Unauthorized();

        await questionService.UpdateQuestionAsync(new UpdateQuestionRequest()
        {
            UpdatedQuestion = new GrpcClient.QuestionDTO()
            {
                QuizId = questionDto.QuizId,
                Index = questionDto.Index,
                Id = questionId,
                Title = questionDto.Title,
            }
        });

        return Ok();
    }

    [HttpDelete("{questionId:int}")]
    public async Task<ActionResult> DeleteQuestion([FromRoute] int questionId)
    {
        if (!await IsAuthorizedToChange(questionId, User)) return Unauthorized();

        await questionService.DeleteQuestionAsync(new DeleteQuestionRequest() { QuestionId = questionId });
        return Ok();
    }

    [HttpGet("{questionId:int}")]
    public async Task<ActionResult<QuestionDTO>> GetQuestion([FromRoute] int questionId)
    {
        var res = await questionService.GetQuestionByIdAsync(new GetQuestionByIdRequest()
            { QuestionId = questionId });

        if (!await IsAuthorizedToAccess(res.Question.QuizId, User)) return Unauthorized();

        return Ok(new QuestionDTO()
        {
            Index = res.Question.Index,
            Title = res.Question.Title,
            QuestionId = res.Question.Id,
            QuizId = res.Question.QuizId,
        });
    }

    private async Task<bool> IsAuthorizedToChange(int questionId, ClaimsPrincipal user)
    {
        var question =
            await questionService.GetQuestionByIdAsync(new GetQuestionByIdRequest { QuestionId = questionId });
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

    private async Task<bool> IsAuthorizedToAccess(int quizId, ClaimsPrincipal user)
    {
        var quiz = (await quizService.GetQuizAsync(new GetQuizRequest { QuizId = quizId })).Quiz;
        int userId = int.Parse(user.FindFirst("Id")!.Value);

        return quiz.CreatorId == userId || quiz.Visibility == "public";
    }
}