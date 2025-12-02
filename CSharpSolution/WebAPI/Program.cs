using Grpc.Net.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<GrpcClient.QuizzesService.QuizzesServiceClient>(sp =>
{
    var channel = GrpcChannel.ForAddress("http://localhost:7042");
    return new GrpcClient.QuizzesService.QuizzesServiceClient(channel);
});

builder.Services.AddScoped<GrpcClient.QuestionService.QuestionServiceClient>(sp =>
{
    var channel = GrpcChannel.ForAddress("http://localhost:7042");
    return new GrpcClient.QuestionService.QuestionServiceClient(channel);
});

builder.Services.AddScoped<GrpcClient.UserService.UserServiceClient>(sp =>
{
    var channel = GrpcChannel.ForAddress("http://localhost:7042");
    return new GrpcClient.UserService.UserServiceClient(channel);
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