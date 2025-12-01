using ApiContracts;

namespace BlazorApp.Services;

public class UserService(HttpClient httpClient) : IUserService
{
    public async Task<UserDTO> CreateUser(string username, string password)
    {
        var response = await httpClient.PostAsJsonAsync("users", new { Username = username, Password = password });
        if (!response.IsSuccessStatusCode) throw new Exception("Kunne ikke oprette. " + response.StatusCode);

        return (await response.Content.ReadFromJsonAsync<UserDTO>())!;
    }
}