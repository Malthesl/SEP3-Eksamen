using Grpc.Net.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using WebAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<GrpcClient.QuizService.QuizServiceClient>(sp =>
{
    var channel = GrpcChannel.ForAddress("http://localhost:7042");
    return new GrpcClient.QuizService.QuizServiceClient(channel);
});

builder.Services.AddSingleton<GrpcClient.QuestionService.QuestionServiceClient>(sp =>
{
    var channel = GrpcChannel.ForAddress("http://localhost:7042");
    return new GrpcClient.QuestionService.QuestionServiceClient(channel);
});

builder.Services.AddSingleton<GrpcClient.UserService.UserServiceClient>(sp =>
{
    var channel = GrpcChannel.ForAddress("http://localhost:7042");
    return new GrpcClient.UserService.UserServiceClient(channel);
});

builder.Services.AddSingleton<GrpcClient.AnswerService.AnswerServiceClient>(sp =>
{
    var channel = GrpcChannel.ForAddress("http://localhost:7042");
    return new GrpcClient.AnswerService.AnswerServiceClient(channel);
});

builder.Services.AddSingleton<GrpcClient.ResultService.ResultServiceClient>(sp =>
{
    var channel = GrpcChannel.ForAddress("http://localhost:7042");
    return new GrpcClient.ResultService.ResultServiceClient(channel);
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "QuizPlusPlus",
            ValidAudience = "Quizzers",
            IssuerSigningKey = new SymmetricSecurityKey("SuperSecretKeyThatIsAtMinimum32CharactersLong"u8.ToArray())
        };
    });

builder.Services.AddSingleton<LiveGameService>();

builder.Services.AddSingleton<AuthorizationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();