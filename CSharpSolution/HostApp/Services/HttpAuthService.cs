using System.Security.Authentication;

namespace HostApp.Services;

public class HttpAuthService(HttpClient httpClient) : IAuthService
{
    public async Task<string> LoginAndReturnToken(string username, string password)
    {
        var response = await httpClient.PostAsJsonAsync("auth/login", new { Username = username, Password = password });
        if (!response.IsSuccessStatusCode) throw new AuthenticationException("Login failed. " + response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
        return result?.Token ?? throw new AuthenticationException("No token received.");
    }
}

public class TokenResponse
{
    public required string Token { get; init; }
}