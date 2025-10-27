using ApiContracts;
using Microsoft.AspNetCore.Mvc;
using GrpcProofOfConceptClient;
using QuestionDTO = ApiContracts.QuestionDTO;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class QuestionsController(ProofOfConceptService.ProofOfConceptServiceClient grpcClient) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<QuestionListDTO>> GetQuestions()
    {
        var response = await grpcClient.GetAllQuestionsAsync(new GetQuestionsRequest());
        
        return Ok(new QuestionListDTO
        {
            Questions = response.Question.Select(q => new QuestionDTO
            {
                Id = q.Id,
                Question = q.Question,
                Answer = q.Answer
            }).ToList()
        });
    }
    
    [HttpGet("{questionId}")]
    public async Task<ActionResult<QuestionDTO>> GetQuestion(int questionId)
    {
        var response = await grpcClient.GetQuestionAsync(new GetQuestionRequest { QuestionId = questionId });
        
        return Ok(new QuestionDTO
        {
            Id = response.Question.Id,
            Question = response.Question.Question,
            Answer = response.Question.Answer
        });
    }
    
    [HttpPost]
    public async Task<ActionResult> CreateQuestion(CreateQuestionDTO question)
    {
        var response = await grpcClient.CreateQuestionAsync(new CreateQuestionRequest
        {
            Question = question.Question,
            Answer = question.Answer
        });
        
        return Created($"/questions/{response.Question.Id}", new QuestionDTO
        {
            Id = response.Question.Id,
            Question = response.Question.Question,
            Answer = response.Question.Answer
        });
    }
}