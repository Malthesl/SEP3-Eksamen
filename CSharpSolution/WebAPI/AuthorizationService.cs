using System.Security.Claims;
using GrpcClient;

namespace WebAPI;

public class AuthorizationService(
    AnswerService.AnswerServiceClient answersService,
    QuestionService.QuestionServiceClient questionService,
    QuizService.QuizServiceClient quizService
)
{
    /// <summary>
    /// Hjælpemetode til at sikre at man har privilege til at ændre en svarmulighed.
    /// </summary>
    /// <param name="answerId">ID'et på svarmuligheden</param>
    /// <param name="user">ClaimsPrincipal på brugeren</param>
    /// <returns>Returner en task, som giver en bool om brugeren har adgang.</returns>
    public async Task<bool> IsAuthorizedToModifyAnswer(int answerId, ClaimsPrincipal user)
    {
        int questionId = (await answersService.GetAnswerAsync(new GetAnswerRequest { Id = answerId })).Answer
            .QuestionId;

        return await IsAuthorizedToModifyQuestion(questionId, user);
    }

    /// <summary>
    /// Hjælpemetode til at sikre at man har privilege til at ændre et spørgsmål.
    /// </summary>
    /// <param name="questionId">ID'et på spørgsmålet</param>
    /// <param name="user">ClaimsPrincipal på brugeren</param>
    /// <returns>Returner en task, som giver en bool om brugeren har adgang.</returns>
    public async Task<bool> IsAuthorizedToModifyQuestion(int questionId, ClaimsPrincipal user)
    {
        int quizId =
            (await questionService.GetQuestionByIdAsync(new GetQuestionByIdRequest { QuestionId = questionId }))
            .Question.QuizId;

        return await IsAuthorizedToModifyQuiz(quizId, user);
    }

    /// <summary>
    /// Hjælpemetode til at sikre at man har privilege til at ændre en quiz.
    /// </summary>
    /// <param name="quizId">ID'et på quiz</param>
    /// <param name="user">ClaimsPrincipal på brugeren</param>
    /// <returns>Returner en task, som giver en bool om brugeren har adgang.</returns>
    public async Task<bool> IsAuthorizedToModifyQuiz(int quizId, ClaimsPrincipal user)
    {
        if (user.Identity?.IsAuthenticated is false or null) return false;
        
        int userId = int.Parse(user.FindFirst("Id")!.Value);
        
        int quizCreatorId = (await quizService.GetQuizAsync(new GetQuizRequest
        {
            QuizId = quizId
        })).Quiz.CreatorId;

        return quizCreatorId == userId;
    }

    /// <summary>
    /// Hjælpemetode til at sikre at man har privilege til at se et spørgsmål.
    /// </summary>
    /// <param name="questionId">ID'et på spørgsmålet</param>
    /// <param name="user">ClaimsPrincipal på brugeren</param>
    /// <returns>Returner en task, som giver en bool om brugeren har adgang.</returns>
    public async Task<bool> IsAuthorizedToAccessQuestion(int questionId, ClaimsPrincipal user)
    {
        int quizId =
            (await questionService.GetQuestionByIdAsync(new GetQuestionByIdRequest { QuestionId = questionId }))
            .Question.QuizId;

        return await IsAuthorizedToAccessQuiz(quizId, user);
    }

    /// <summary>
    /// Hjælpemetode til at sikre at man har privilege til at se en quiz.
    /// </summary>
    /// <param name="quizId">ID'et på quiz</param>
    /// <param name="user">ClaimsPrincipal på brugeren</param>
    /// <returns>Returner en task, som giver en bool om brugeren har adgang.</returns>
    public async Task<bool> IsAuthorizedToAccessQuiz(int quizId, ClaimsPrincipal user)
    {
        var quiz = (await quizService.GetQuizAsync(new GetQuizRequest { QuizId = quizId })).Quiz;
        
        if (quiz.Visibility == "public") return true;
        
        if (user.Identity?.IsAuthenticated is false or null) return false;
        
        int userId = int.Parse(user.FindFirst("Id")!.Value);

        return quiz.CreatorId == userId;
    }
}