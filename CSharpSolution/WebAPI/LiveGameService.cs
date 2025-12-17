using GrpcClient;

namespace WebAPI;

public class LiveGameService(
    QuizService.QuizServiceClient quizService,
    QuestionService.QuestionServiceClient questionService,
    AnswerService.AnswerServiceClient answerService,
    ResultService.ResultServiceClient resultService)
{
    private Dictionary<string, LiveGame> Games { get; } = new();

    public LiveGame? GetGame(string gameId) => Games.GetValueOrDefault(gameId);

    public LiveGame CreateGame(int quizId, int hostId)
    {
        var quiz = quizService.GetQuiz(new GetQuizRequest { QuizId = quizId }).Quiz;
        var questions = questionService
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
                        IsCorrect = a.IsCorrect,
                        Index = a.Index
                    })
                    .ToList()
            })
            .ToList();

        LiveGame game = new LiveGame(hostId, quiz, questions, resultService);

        Games.Add(game.GameId, game);

        return game;
    }

    public LiveGame? GetGameFromJoinCode(string joinCode) => Games.Values.FirstOrDefault(g => g.JoinCode == joinCode);
}

public class LiveGame
{
    public string GameId { get; } = Guid.NewGuid().ToString();
    public int HostUserId { get; }
    public int QuizId { get; }
    public string JoinCode { get; }

    private const int QuestionTime = 10 * 1000;

    public int CurrentQuestionId { get; private set; }

    public LiveGameQuestion? CurrentQuestion => Questions.Find(q => q.QuestionId == CurrentQuestionId);

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

    public QuizDTO Quiz { get; }

    public List<LiveGameQuestion> Questions { get; }

    public List<LiveGamePlayer> Players { get; } = [];

    public long NextEventTime { get; private set; }

    public int UpdateNo { get; private set; } = 1;

    private readonly List<TaskCompletionSource> _tasks = [];

    private ResultService.ResultServiceClient _resultService;

    public LiveGame(
        int userId,
        QuizDTO quiz,
        List<LiveGameQuestion> questions,
        ResultService.ResultServiceClient resultService)
    {
        HostUserId = userId;
        QuizId = quiz.Id;

        JoinCode = GenerateJoinCode();

        Quiz = quiz;
        Questions = questions;

        _resultService = resultService;
    }

    /// <summary>
    /// Henter den nuværende game, efter en ændring er sket.
    /// </summary>
    public async Task<LiveGame> GetGameState(int lastUpdateNo)
    {
        if (UpdateNo != lastUpdateNo) return this;

        TaskCompletionSource t;

        lock (_tasks)
        {
            t = new TaskCompletionSource();
            _tasks.Add(t);
        }

        await Task.WhenAny(t.Task, Task.Delay(1000 * 30));

        return this;
    }

    /// <summary>
    /// Kald denne metode, når en ændring er sket. Så bliver klienterne opdateret!
    /// </summary>
    private void StateUpdated()
    {
        lock (_tasks)
        {
            UpdateNo++;

            foreach (var task in _tasks)
            {
                task.SetResult();
            }

            _tasks.Clear();
        }
    }

    /// <summary>
    /// Join Game
    /// </summary>
    public LiveGamePlayer JoinGame(string name)
    {
        LiveGamePlayer player = new LiveGamePlayer { PlayerId = Guid.NewGuid().ToString(), Name = name };

        lock (Players)
        {
            Players.Add(player);
        }

        StateUpdated();

        return player;
    }

    private static readonly ISet<string> JoinCodesInUse = new HashSet<string>();

    /// <summary>
    /// Hjælpemetode til at generere join codes
    /// </summary>
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

    public void Start()
    {
        if (CurrentState != "lobby") return;

        SetQuestion(0);
    }

    public void Continue()
    {
        switch (CurrentState)
        {
            case "question-answering":
                SetStateAnswer(CurrentQuestionId);
                break;
            case "question-answer":
                SetStateLeaderboard(CurrentQuestionId);
                break;
            case "leaderboard":
                NextQuestion(CurrentQuestionId);
                break;
            default:
                // staten kan ikke skippes
                break;
        }
    }

    private void SetQuestion(int index)
    {
        CurrentState = "question-countdown";
        int questionId = Questions[index].QuestionId;
        CurrentQuestionId = questionId;

        foreach (var player in Players)
        {
            player.LatestAnswerId = null;
            player.LatestScoreChange = null;
        }

        UpdateStateAndCountdown(3 * 1000, () => SetStateAnswering(questionId));
    }

    private void SetStateAnswering(int questionId)
    {
        if (CurrentState != "question-countdown" || CurrentQuestionId != questionId) return;

        CurrentState = "question-answering";

        _questionStartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        UpdateStateAndCountdown(QuestionTime, () => SetStateAnswer(questionId));
    }

    private void SetStateAnswer(int questionId)
    {
        if (CurrentState != "question-answering" || CurrentQuestionId != questionId) return;

        CurrentState = "question-answer";

        StateUpdated();
    }

    private void SetStateLeaderboard(int questionId)
    {
        if (CurrentState != "question-answer" || CurrentQuestionId != questionId) return;

        int index = Questions.FindIndex(q => q.QuestionId == questionId);

        if (index + 1 >= Questions.Count)
        {
            FinishQuiz();
        }
        else
        {
            CurrentState = "leaderboard";
            UpdateStateAndCountdown(5 * 1000, () => NextQuestion(questionId));
        }
    }

    private void NextQuestion(int questionId)
    {
        if (CurrentState != "leaderboard" || CurrentQuestionId != questionId) return;

        int index = Questions.FindIndex(q => q.QuestionId == questionId);

        SetQuestion(index + 1);
    }

    private async Task UpdateStateAndCountdown(int msDelay, Action callback)
    {
        NextEventTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + msDelay;

        StateUpdated();

        await Task.Delay(msDelay);

        callback();
    }

    private async Task FinishQuiz()
    {
        CurrentState = "game-over";
        StateUpdated();

        var results = new SubmitGameResultsRequest
        {
            Game = new GameDTO
            {
                HostId = HostUserId,
                Id = GameId,
                PlayedTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                QuizId = QuizId
            }
        };

        results.Results.AddRange(Players.Select(p =>
        {
            var participant = new SubmitResultsByParticipant
            {
                Name = p.Name,
                Score = p.Score
            };

            participant.Answers.AddRange(p.Answers.Select(a => new ParticipantResult
            {
                AnswerId = a.AnswerId
            }));

            return participant;
        }));

        // Submit resultater
        await _resultService.SubmitGameResultsAsync(results);
    }

    private long _questionStartTime = 0;

    public void Answer(int questionId, int answerId, string playerId)
    {
        LiveGameQuestion? question = CurrentQuestion;
        if (question is null) throw new Exception("Question not found");
        LiveGameAnswer? answer = question.Answers.Find(a => a.AnswerId == answerId);
        if (answer is null) throw new Exception("Answer not found");
        LiveGamePlayer? player = Players.Find(p => p.PlayerId == playerId);
        if (player is null) throw new Exception("Player not found");

        if (player.Answers.Any(a => a.QuestionId == questionId))
            throw new Exception("Player already answered question");

        lock (player)
        {
            player.Answers.Add(new LiveGamePlayerAnswer { QuestionId = questionId, AnswerId = answer.AnswerId });

            long timeElapsed = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - _questionStartTime;
            long timeLeft = QuestionTime - timeElapsed;
            double timePct = (double)timeLeft / QuestionTime;

            int score = answer.IsCorrect ? (int)(250 + 750 * timePct) : 0;

            player.Score += score;
            player.LatestAnswerId = answer.AnswerId;
            player.LatestScoreChange = score;

            if (Players.All(p => p.LatestAnswerId is not null)) SetStateAnswer(questionId);
            else StateUpdated();
        }
    }
}

public class LiveGamePlayer
{
    public required string PlayerId { get; init; } = Guid.NewGuid().ToString();
    public required string Name { get; init; }

    public int Score { get; set; }

    public List<LiveGamePlayerAnswer> Answers { get; init; } = [];

    public int? LatestAnswerId { get; set; }
    public int? LatestScoreChange { get; set; }
}

public class LiveGamePlayerAnswer
{
    public required int QuestionId { get; init; }
    public required int AnswerId { get; init; }
}

public class LiveGameQuestion
{
    public required int QuestionId { get; init; }
    public required String Title { get; init; }
    public required List<LiveGameAnswer> Answers { get; init; }
}

public class LiveGameAnswer
{
    public required int AnswerId { get; init; }
    public required String Title { get; init; }
    public required bool IsCorrect { get; init; }
    public required int Index { get; init; }
}