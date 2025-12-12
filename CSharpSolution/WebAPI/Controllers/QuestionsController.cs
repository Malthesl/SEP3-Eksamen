using ApiContracts;
using QuestionDTO = ApiContracts.QuestionDTO;
using GrpcClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// WebAPI til at håndtere spørgsmålene i quizzer.
/// </summary>
[Authorize]
[ApiController]
[Route("[controller]")]
public class QuestionsController(
    QuestionService.QuestionServiceClient questionService,
    AuthorizationService authService) : ControllerBase
{
    /// <summary>
    /// Henter alle spørgsmål i en quiz.
    /// Sikkerhed: Hvis quizzen er privat, kræver dette endpoint at man er logget ind som quiz-ejeren.
    /// </summary>
    /// <param name="quizId">ID'et på quizzen</param>
    /// <returns>En liste af spørgsmål som QuestionDTO'er</returns>
    [HttpGet]
    public async Task<ActionResult<List<QuestionDTO>>> GetAllQuestionsInQuiz([FromQuery] int quizId)
    {
        if (!await authService.IsAuthorizedToAccessQuiz(quizId, User)) return Unauthorized();

        var questionsDto = await questionService.GetAllQuestionsInQuizAsync(new GetAllQuestionsInQuizRequest
        {
            QuizId = quizId
        });

        List<QuestionDTO> dtos = questionsDto.Questions.Select(question => new QuestionDTO
            {
                QuestionId = question.Id, Title = question.Title, QuizId = question.QuizId, Index = question.Index,
            })
            .ToList();

        return Ok(dtos);
    }

    /// <summary>
    /// Tilføjer et spørgsmål til en quiz.
    /// Sikkerhed: Kræver at man er logget ind som quiz-ejeren.
    /// </summary>
    /// <param name="createDto">DTO med informationer om titlen og id'et på quizzen</param>
    /// <returns>Det nye spørgsmål</returns>
    [HttpPost]
    public async Task<ActionResult<QuestionDTO>> AddQuestion([FromBody] CreateQuestionDTO createDto)
    {
        if (!await authService.IsAuthorizedToModifyQuiz(createDto.QuizId, User)) return Unauthorized();
        
        if (String.IsNullOrWhiteSpace(createDto.Title)) return BadRequest("Title må ikke være tom");

        var res = await questionService.AddQuestionAsync(new AddQuestionRequest
        {
            QuizId = createDto.QuizId,
            Title = createDto.Title,
        });

        return Created($"/questions/{res.NewQuestion.Id}", new QuestionDTO
        {
            Index = res.NewQuestion.Index,
            Title = res.NewQuestion.Title,
            QuestionId = res.NewQuestion.Id,
            QuizId = res.NewQuestion.QuizId,
        });
    }

    /// <summary>
    /// Opdaterer et spørgsmål.
    /// Sikkerhed: Kræver at man er logget ind som quiz-ejeren.
    /// </summary>
    /// <param name="questionId">ID'et på spørgsmålet</param>
    /// <param name="questionDto">DTO med informationer om ændringen</param>
    /// <returns>QuestionDTO med resultatet af de nye ændringer</returns>
    [HttpPost("{questionId:int}")]
    public async Task<ActionResult<QuestionDTO>> UpdateQuestion([FromRoute] int questionId,
        [FromBody] QuestionDTO questionDto)
    {
        if (!await authService.IsAuthorizedToModifyQuestion(questionDto.QuestionId, User)) return Unauthorized();
        
        if (String.IsNullOrWhiteSpace(questionDto.Title)) return BadRequest("Titlen må ikke være tom");

        await questionService.UpdateQuestionAsync(new UpdateQuestionRequest
        {
            UpdatedQuestion = new GrpcClient.QuestionDTO
            {
                QuizId = questionDto.QuizId,
                Index = questionDto.Index,
                Id = questionId,
                Title = questionDto.Title,
            }
        });

        return await GetQuestion(questionId);
    }

    /// <summary>
    /// Sletter et spørgsmål.
    /// Sikkerhed: Kræver at man er logget ind som quiz-ejeren.
    /// </summary>
    /// <param name="questionId">ID'et på spørgsmålet</param>
    /// <returns>Ok hvis spørgsmålet blev slettet</returns>
    [HttpDelete("{questionId:int}")]
    public async Task<ActionResult> DeleteQuestion([FromRoute] int questionId)
    {
        if (!await authService.IsAuthorizedToModifyQuestion(questionId, User)) return Unauthorized();

        await questionService.DeleteQuestionAsync(new DeleteQuestionRequest { QuestionId = questionId });
        
        return Ok();
    }

    /// <summary>
    /// Henter et spørgsmål.
    /// Sikkerhed: Hvis quizzen er privat, kræver dette endpoint at man er logget ind som quiz-ejeren.
    /// </summary>
    /// <param name="questionId">ID'et på spørgsmålet</param>
    /// <returns>Et QuestionDTO med spørgsmålets informationer</returns>
    [HttpGet("{questionId:int}")]
    public async Task<ActionResult<QuestionDTO>> GetQuestion([FromRoute] int questionId)
    {
        var res = await questionService.GetQuestionByIdAsync(new GetQuestionByIdRequest
            { QuestionId = questionId });

        if (!await authService.IsAuthorizedToAccessQuiz(res.Question.QuizId, User)) return Unauthorized();

        return Ok(new QuestionDTO
        {
            Index = res.Question.Index,
            Title = res.Question.Title,
            QuestionId = res.Question.Id,
            QuizId = res.Question.QuizId,
        });
    }
}