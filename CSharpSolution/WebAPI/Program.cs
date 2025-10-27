using Grpc.Net.Client;
using GrpcProofOfConceptClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ProofOfConceptService.ProofOfConceptServiceClient>(sp =>
{
    using var channel = GrpcChannel.ForAddress("https://localhost:7042");
    var client = new ProofOfConceptService.ProofOfConceptServiceClient(channel);
    return client;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();