using System.Security.Claims;
using AnswerDTO = ApiContracts.AnswerDTO;
using ApiContracts;
using GrpcClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class AnswersController(
    AnswersService.AnswersServiceClient answersService,
    QuestionService.QuestionServiceClient questionService,
    QuizzesService.QuizzesServiceClient quizService) : ControllerBase
{
    [HttpGet("FromQuestion/{questionId:int}")]
    public async Task<ActionResult<List<AnswerDTO>>> GetAnswers([FromRoute] int questionId)
    {
        try
        {
            if (!await IsAuthorizedToAccess(questionId, User)) return Unauthorized();
            
            var answers = await answersService.GetAllAnswersInQuestionAsync(new GetAllAnswersInQuestionRequest()
            {
                QuestionId = questionId
            });

            List<AnswerDTO> returnList = new List<AnswerDTO>();
            foreach (var answer in answers.Answers)
            {
                returnList.Add(new AnswerDTO()
                {
                    Index = answer.Index,
                    Title = answer.Title,
                    AnswerId = answer.Id,
                    IsCorrect = answer.IsCorrect,
                    QuestionId = answer.QuestionId,
                });
            }
            
            return Ok(returnList);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<AnswerDTO>> AddAnswer([FromBody] CreateAnswerDTO createAnswerDto)
    {
        try
        {
            if (!await IsAuthorizedToAdd(createAnswerDto.QuestionId, User)) return Unauthorized(); 
            
            var res = await answersService.AddAnswerAsync(new AddAnswerRequest()
            {  
                Index = createAnswerDto.Index,
                Title = createAnswerDto.Title,
                IsCorrect = createAnswerDto.IsCorrect,
                QuestionId = createAnswerDto.QuestionId,
            });
            
            return Ok(new AnswerDTO()
            {
                Index = res.Answer.Index,
                Title = res.Answer.Title,
                AnswerId = res.Answer.Id,
                IsCorrect = res.Answer.IsCorrect,
                QuestionId = res.Answer.QuestionId,
            });
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("{answerId:int}")]
    public async Task<ActionResult> UpdateAnswer([FromRoute] int answerId, [FromBody] AnswerDTO answerDto)
    {
        try
        {
            if (!await IsAuthorizedToChange(answerId, User))
            
            answersService.UpdateAnswer(new UpdateAnswerRequest()
            {
                NewAnswer = new GrpcClient.AnswerDTO()
                {
                    Index = answerDto.Index,
                    Title = answerDto.Title,
                    Id = answerId,
                    IsCorrect = answerDto.IsCorrect,
                    QuestionId = answerDto.QuestionId
                }
            });
            
            return Ok(answerDto);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{answerId:int}")]
    public async Task<ActionResult> DeleteAnswer([FromRoute] int answerId)
    {
        try
        {
            if (!await IsAuthorizedToChange(answerId, User)) return Unauthorized();
            
            answersService.DeleteAnswer(new DeleteAnswerRequest()
            {
                AnswerId = answerId
            });

            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    private async Task<bool> IsAuthorizedToChange(int answerId, ClaimsPrincipal user)
    {
        int questionId = (await answersService.GetAnswerAsync(new GetAnswerRequest { Id = answerId })).Answer.QuestionId;
        var question = await questionService.GetQuestionByIdAsync(new GetQuestionByIdRequest{QuestionId = questionId});
        int userId = int.Parse(user.FindFirst("Id")!.Value);
        int quizCreatorId = (await quizService.GetQuizAsync(new GetQuizRequest()
        {
            QuizId = question.Question.QuizId
        })).Quiz.CreatorId;
        
        return quizCreatorId == userId;
    }

    private async Task<bool> IsAuthorizedToAdd(int questionId, ClaimsPrincipal user)
    {
        int userId = int.Parse(user.FindFirst("Id")!.Value);
        int quizId = (await questionService.GetQuestionByIdAsync(new GetQuestionByIdRequest { QuestionId = questionId })).Question.QuizId;
        int quizCreatorId = (await quizService.GetQuizAsync(new GetQuizRequest()
        {
            QuizId = quizId
        })).Quiz.CreatorId;
        
        return quizCreatorId == userId;
    }
    
    private async Task<bool> IsAuthorizedToAccess(int questionId, ClaimsPrincipal user)
    {
        int quizId = (await questionService.GetQuestionByIdAsync(new GetQuestionByIdRequest { QuestionId = questionId })).Question.QuizId;
        var quiz = (await quizService.GetQuizAsync(new GetQuizRequest{QuizId = quizId})).Quiz;
        int userId = int.Parse(user.FindFirst("Id")!.Value);
        
        return quiz.CreatorId == userId || quiz.Visibility == "public";
    }
}