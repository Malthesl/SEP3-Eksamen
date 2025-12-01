using System.Security.Authentication;
using ApiContracts;

namespace BlazorClient.Services;

public class AuthService(HttpClient httpClient)
{
    public async Task<string> LoginAndReturnToken(string username, string password)
    {
        var response = await httpClient.PostAsJsonAsync("auth/login", new { Username = username, Password = password });
        if (!response.IsSuccessStatusCode) throw new AuthenticationException("Login failed. " + response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
        return result?.Token ?? throw new AuthenticationException("No token received.");
    }

    public async Task<UserDTO> RegisterAccount(string username, string password)
    {
        var response = await httpClient.PostAsJsonAsync("users", new { Username = username, Password = password });
        if (!response.IsSuccessStatusCode) throw new Exception("Kunne ikke oprette. " + response.StatusCode);

        return (await response.Content.ReadFromJsonAsync<UserDTO>())!;
    }
}

public class TokenResponse
{
    public required string Token { get; init; }
}