using ApiContracts;
using GrpcClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizDTO = ApiContracts.QuizDTO;
using UserDTO = ApiContracts.UserDTO;

namespace WebAPI.Controllers;

/// <summary>
/// WebAPI til at håndtere quizzer.
/// </summary>
[ApiController]
[Route("[controller]")]
public class QuizzesController(QuizService.QuizServiceClient quizService, AuthorizationService authService) : ControllerBase
{
    /// <summary>
    /// Opretter en ny quiz.
    /// Sikkerhed: Man skal være logget ind for at oprette en quiz.
    /// </summary>
    /// <param name="createDto">DTO med oplysninger om quizzen (titel)</param>
    /// <returns>QuizDTO med den nye quiz</returns>
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<QuizDTO>> CreateQuiz([FromBody] CreateQuizDTO createDto)
    {
        int userId = int.Parse(User.FindFirst("Id")!.Value);

        var res = await quizService.AddQuizAsync(new AddQuizRequest
        {
            Title = createDto.Title,
            CreatorId = userId
        });

        return Created($"/quiz/{res.QuizDto.Id}", new QuizDTO
        {
            Id = res.QuizDto.Id,
            Title = res.QuizDto.Title,
            Visibility = res.QuizDto.Visibility,
            CreatorId = res.QuizDto.CreatorId
        });
    }

    /// <summary>
    /// Opdaterer en quiz.
    /// Sikkerhed: Kræver at man er logget ind som quiz-ejeren.
    /// </summary>
    /// <param name="quizId">ID'et på quizzen der skal ændres</param>
    /// <param name="quizChanges">DTO med oplysninger ændringer i quizzen (titel, synlighed)</param>
    /// <returns>QuizDTO med de nye ændringer i quizzen</returns>
    [Authorize]
    [HttpPost("{quizId:int}")]
    public async Task<ActionResult<QuizDTO>> UpdateQuiz([FromRoute] int quizId, [FromBody] UpdateQuizDTO quizChanges)
    {
        if (!await authService.IsAuthorizedToModifyQuiz(quizId, User)) return Unauthorized();
        
        await quizService.UpdateQuizAsync(new UpdateQuizRequest
        {
            Quiz = new GrpcClient.QuizDTO
            {
                Title = quizChanges.Title,
                Visibility = quizChanges.Visibility,
                Id = quizId
            }
        });

        return await GetQuiz(quizId);
    }

    /// <summary>
    /// Sletter en quiz.
    /// Sikkerhed: Kræver at man er logget ind som quiz-ejeren.
    /// </summary>
    /// <param name="quizId">ID'et på quizzen der skal slettes</param>
    /// <returns>Ok hvis quizzen blev slettet</returns>
    [HttpDelete("{quizId:int}")]
    public async Task<ActionResult> DeleteQuiz([FromRoute] int quizId)
    {
        if (!await authService.IsAuthorizedToModifyQuiz(quizId, User)) return Unauthorized();
        
        await quizService.DeleteQuizAsync(new DeleteQuizRequest { QuizId = quizId });
        
        return Ok();
    }

    /// <summary>
    /// Henter en quiz.
    /// Sikkerhed: Hvis quizzen er privat, kræver dette endpoint at man er logget ind som quiz-ejeren.
    /// </summary>
    /// <param name="quizId">ID'et på quizzen</param>
    /// <returns>QuizDTO med oplysninger om quizzen</returns>
    [HttpGet("{quizId:int}")]
    public async Task<ActionResult<QuizDTO>> GetQuiz([FromRoute] int quizId)
    {
        if (!await authService.IsAuthorizedToAccessQuiz(quizId, User)) return Unauthorized();
        
        var res = await quizService.GetQuizAsync(new GetQuizRequest
        {
            QuizId = quizId
        });
        
        return Ok(new QuizDTO
        {
            Id = res.Quiz.Id,
            Title = res.Quiz.Title,
            Visibility = res.Quiz.Visibility,
            CreatorId = res.Quiz.CreatorId,
            Creator = new UserDTO
            {
                Id = res.Quiz.CreatorId,
                Username = res.Quiz.Creator.Username
            },
            QuestionCount = res.Quiz.QuestionCount
        });
    }

    /// <summary>
    /// Henter en liste af quizzer efter forskellige filtre.
    /// </summary>
    /// <param name="query">Søgeord</param>
    /// <param name="visibility">Vis kun quizzer med ønsket synligheder (komma separeret streng)</param>
    /// <param name="creatorId">Vis kun quizzer lavet af denne bruger</param>
    /// <param name="start">Vis resultater fra dette indeks</param>
    /// <param name="count">Vis dette antal resultater</param>
    /// <returns>En QuizQueryDTO der indeholder en liste af resultaterne</returns>
    [HttpGet]
    public async Task<ActionResult<QuizQueryDTO>> QueryMany([FromQuery] string? query, [FromQuery] string? visibility,
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

        var res = await quizService.QueryQuizzesAsync(req);

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