using HostApp.Components;
using HostApp.Services;
using HostApp.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("keys"))
    .SetApplicationName("HostApp");

builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, TokenAuthenticationStateProvider>();
builder.Services.AddScoped<HttpAuthService>();
builder.Services.AddScoped<AuthTokenHandler>();

builder.Services.AddScoped(sp =>
{
    var handler = sp.GetRequiredService<AuthTokenHandler>();
    handler.InnerHandler = new HttpClientHandler();
    
    return new HttpClient(handler)
    {
        BaseAddress = new Uri("http://localhost:5171")
    };
});

builder.Services.AddScoped<IUserService, HttpUserService>();
builder.Services.AddScoped<IQuizService, HttpQuizService>();
builder.Services.AddScoped<IQuestionService, HttpQuestionService>();
builder.Services.AddScoped<IAnswerService, HttpAnswerService>();
builder.Services.AddScoped<IQuizHostService, HttpQuizHostService>();
builder.Services.AddScoped<IGameResultsService, HttpGameResultsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();