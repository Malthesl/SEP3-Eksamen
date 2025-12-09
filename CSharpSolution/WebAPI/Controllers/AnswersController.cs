using AnswerDTO = ApiContracts.AnswerDTO;
using ApiContracts;
using GrpcClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// WebAPI til at håndtere svarmulighederne til et spørgsmål.
/// </summary>
[Authorize]
[ApiController]
[Route("[controller]")]
public class AnswersController(AnswerService.AnswerServiceClient answersService, AuthorizationService authService)
    : ControllerBase
{
    /// <summary>
    /// Henter alle svarmuligheder der hører til et spørgsmål.
    /// Sikkerhed: Hvis quizzen spørgsmålet hører til er privat, kræver dette endpoint at man er logget ind som quiz-ejeren.
    /// </summary>
    /// <param name="questionId">ID'et på spørgsmålet</param>
    /// <returns>En liste af svarmuligheder som AnswerDTO'er</returns>
    [HttpGet]
    public async Task<ActionResult<List<AnswerDTO>>> GetAnswers([FromQuery] int questionId)
    {
        if (!await authService.IsAuthorizedToAccessQuestion(questionId, User)) return Unauthorized();

        var answers = await answersService.GetAllAnswersInQuestionAsync(new GetAllAnswersInQuestionRequest
        {
            QuestionId = questionId
        });

        List<AnswerDTO> dtos = answers.Answers.Select(answer => new AnswerDTO
            {
                Index = answer.Index,
                Title = answer.Title,
                AnswerId = answer.Id,
                IsCorrect = answer.IsCorrect,
                QuestionId = answer.QuestionId,
            })
            .ToList();

        return Ok(dtos);
    }

    /// <summary>
    /// Tilføjer en ny svarmulighed til en quiz.
    /// Sikkerhed: Kræver at man er logget ind som quiz-ejeren.
    /// </summary>
    /// <param name="createDto">DTO med informationer om titlen, rækkefølge indeks, id på spørgsmålet og om svaret er korrekt</param>
    /// <returns>Den nye svarmulighed</returns>
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<AnswerDTO>> AddAnswer([FromBody] CreateAnswerDTO createDto)
    {
        if (!await authService.IsAuthorizedToModifyQuestion(createDto.QuestionId, User)) return Unauthorized();

        var res = await answersService.AddAnswerAsync(new AddAnswerRequest
        {
            Index = createDto.Index,
            Title = createDto.Title,
            IsCorrect = createDto.IsCorrect,
            QuestionId = createDto.QuestionId,
        });

        return Created("/answers/" + res.Answer.Id, new AnswerDTO
        {
            Index = res.Answer.Index,
            Title = res.Answer.Title,
            AnswerId = res.Answer.Id,
            IsCorrect = res.Answer.IsCorrect,
            QuestionId = res.Answer.QuestionId,
        });
    }

    /// <summary>
    /// Opdaterer en eksisterende svarmulighed.
    /// Sikkerhed: Kræver at man er logget ind som quiz-ejeren.
    /// </summary>
    /// <param name="answerId">ID'et på svarmuligheden der skal opdateres</param>
    /// <param name="answerDto">DTO med ændringerne til indeks, titel og korrekthed</param>
    /// <returns>AnswerDTO med resultatet af de nye ændringer</returns>
    [Authorize]
    [HttpPost("{answerId:int}")]
    public async Task<ActionResult<AnswerDTO>> UpdateAnswer([FromRoute] int answerId, [FromBody] AnswerDTO answerDto)
    {
        if (!await authService.IsAuthorizedToModifyAnswer(answerId, User)) return Unauthorized();

        await answersService.UpdateAnswerAsync(new UpdateAnswerRequest
        {
            NewAnswer = new GrpcClient.AnswerDTO
            {
                Index = answerDto.Index,
                Title = answerDto.Title,
                Id = answerId,
                IsCorrect = answerDto.IsCorrect,
                QuestionId = answerDto.QuestionId
            }
        });

        return Ok((await answersService.GetAnswerAsync(new GetAnswerRequest { Id = answerId })).Answer);
    }

    /// <summary>
    /// Sletter en svarmulighed.
    /// Sikkerhed: Kræver at man er logget ind som quiz-ejeren.
    /// </summary>
    /// <param name="answerId">ID'et på spørgsmålet der skal opdateres</param>
    /// <returns>Ok hvis svaret er blevet slettet korrekt</returns>
    [Authorize]
    [HttpDelete("{answerId:int}")]
    public async Task<ActionResult> DeleteAnswer([FromRoute] int answerId)
    {
        if (!await authService.IsAuthorizedToModifyAnswer(answerId, User)) return Unauthorized();

        await answersService.DeleteAnswerAsync(new DeleteAnswerRequest { AnswerId = answerId });

        return Ok();
    }
}