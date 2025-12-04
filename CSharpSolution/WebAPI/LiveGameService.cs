using GrpcClient;

namespace WebAPI;

public class LiveGameService(
    QuizService.QuizServiceClient quizService,
    QuestionService.QuestionServiceClient questionService,
    AnswerService.AnswerServiceClient answerService)
{
    private Dictionary<string, LiveGame> Games { get; } = new();

    public LiveGame? GetGame(string gameId) => Games.GetValueOrDefault(gameId);

    public LiveGame CreateGame(int quizId, int userId)
    {
        LiveGame game = new LiveGame(quizId, userId, quizService, questionService, answerService);

        Games.Add(game.GameId, game);

        return game;
    }
}

public class LiveGame
{
    public string GameId { get; init; } = Guid.NewGuid().ToString();
    public int HostUserId { get; init; }
    public int QuizId { get; init; }
    public string JoinCode { get; init; }
    
    public int CurrentQuestionId { get; private set; }

    /// <summary>
    /// Det nuværende state af quizzen.
    /// Tænker umiddelbare typer er:
    /// - "lobby": Spillet er ikke startet og spillere kan joine
    /// - "question-countdown": Et spørgsmål bliver vist på værtens skærm (fra CurrentQuestionId)
    /// - "question-answering": Deltagere kan nu svare på spørgsmålet
    /// - "question-answer": Deltagere kan ikke længere svare, det korrekte svar bliver vist
    /// - "leaderboard": Imellem questions bliver leaderboardet vist og deltagerne kan se deres rankering
    /// - "game-over": Quizzen er afsluttet, og vinderen bliver vist
    /// </summary>
    public string CurrentState { get; private set; } = "lobby";

    public QuizDTO Quiz { get; init; }

    public List<LiveGameQuestion> Questions { get; init; }

    public List<LiveGamePlayer> Players { get; init; } = [];

    private readonly List<TaskCompletionSource> _tasks = [];

    public LiveGame(int quizId,
        int userId,
        QuizService.QuizServiceClient quizService,
        QuestionService.QuestionServiceClient questionService,
        AnswerService.AnswerServiceClient answerService)
    {
        HostUserId = userId;
        QuizId = quizId;

        JoinCode = GenerateJoinCode();

        Quiz = quizService.GetQuiz(new GetQuizRequest { QuizId = quizId }).Quiz;
        Questions = questionService
            .GetAllQuestionsInQuiz(new GetAllQuestionsInQuizRequest { QuizId = quizId })
            .Questions
            .Select(q => new LiveGameQuestion
            {
                QuestionId = q.Id,
                Title = q.Title,
                Answers = answerService
                    .GetAllAnswersInQuestion(new GetAllAnswersInQuestionRequest { QuestionId = q.Id })
                    .Answers
                    .Select(a => new LiveGameAnswer
                    {
                        AnswerId = a.Id,
                        Title = a.Title,
                        IsCorrect = a.IsCorrect
                    })
                    .ToList()
            })
            .ToList();
    }

    public async Task<LiveGame> GetGameState(bool force = false)
    {
        if (force) return this;
        var t = new TaskCompletionSource();
        _tasks.Add(t);
        await Task.WhenAny(t.Task, Task.Delay(1000 * 30));
        return this;
    }

    public void StateUpdated()
    {
        foreach (var task in _tasks)
        {
            task.SetResult();
        }

        _tasks.Clear();
    }

    private static readonly ISet<string> JoinCodesInUse = new HashSet<string>();

    private static String GenerateJoinCode()
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var result = new char[6];
        string joinCode;

        do
        {
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = chars[Random.Shared.Next(chars.Length)];
            }

            joinCode = new string(result);
            
        } while (JoinCodesInUse.Contains(joinCode));

        JoinCodesInUse.Add(joinCode);
        
        return joinCode;
    }
}

public class LiveGamePlayer
{
    public required string PlayerId { get; init; } = Guid.NewGuid().ToString();
}

public class LiveGameQuestion
{
    public required int QuestionId { get; set; }
    public required String Title { get; set; }
    public required List<LiveGameAnswer> Answers { get; set; }
}

public class LiveGameAnswer
{
    public required int AnswerId { get; set; }
    public required String Title { get; set; }
    public required bool IsCorrect { get; set; }
}