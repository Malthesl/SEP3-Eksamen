using AnswerDTO = ApiContracts.AnswerDTO;
using ApiContracts;
using GrpcClient;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AnswersController(AnswersService.AnswersServiceClient answersService) : ControllerBase
{
    [HttpGet("FromQuestion/{questionId:int}")]
    public async Task<ActionResult<List<AnswerDTO>>> GetAnswers([FromRoute] int questionId)
    {
        try
        {
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
            answersService.UpdateAnswer(new UpdateAnswerRequest()
            {
                NewAnswer = new GrpcClient.AnswerDTO()
                {
                    Index = answerDto.Index,
                    Title = answerDto.Title,
                    Id = answerDto.AnswerId,
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
}