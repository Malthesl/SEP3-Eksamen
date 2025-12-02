using ApiContracts;
using GrpcClient;
using Microsoft.AspNetCore.Mvc;
using QuestionDTO = ApiContracts.QuestionDTO;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class QuizController(QuizzesService.QuizzesServiceClient quizService) : ControllerBase
{
}