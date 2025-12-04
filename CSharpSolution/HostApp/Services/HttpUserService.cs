using ApiContracts;

namespace HostApp.Services;

public class HttpUserService(HttpClient httpClient) : IUserService
{
    public async Task<UserDTO> CreateUser(string username, string password)
    {
        var response = await httpClient.PostAsJsonAsync("users", new { Username = username, Password = password });
        if (!response.IsSuccessStatusCode) throw new Exception("Kunne ikke oprette. " + response.StatusCode);

        return (await response.Content.ReadFromJsonAsync<UserDTO>())!;
    }

    public async Task<UserDTO> GetUser(int id)
    {
        var response = await httpClient.GetAsync($"users/{id}");
        if (!response.IsSuccessStatusCode) throw new Exception("kunne ikke hente bruger. " + response.StatusCode);

        return (await response.Content.ReadFromJsonAsync<UserDTO>())!;
    }
    public async Task<UserDTO> UpdateUsername(int userId, string username)
    {
        var res = await httpClient.PostAsJsonAsync($"/users/{userId}", new { Username = username});
        if (!res.IsSuccessStatusCode) throw new Exception("Kunne ikke opdatere brugernavn. " + res.StatusCode);

        return (await res.Content.ReadFromJsonAsync<UserDTO>())!;
    }

    public async Task<UserDTO> UpdatePassword(int userId, string newPassword)
    {
        var res = await  httpClient.PostAsJsonAsync($"/users/{userId}", new { Password = newPassword});
        if (!res.IsSuccessStatusCode) throw new Exception("kunne ikke opdatere adgangskode. " + res.StatusCode);

        return (await res.Content.ReadFromJsonAsync<UserDTO>())!;
    }

    public async Task<UserDTO> UpdateUser(UserDTO user)
    {
        var response = await httpClient.PostAsJsonAsync("users", user);
        if (!response.IsSuccessStatusCode) throw new Exception("Kunne ikke opdatere bruger. " + response.StatusCode);
        
        return (await response.Content.ReadFromJsonAsync<UserDTO>())!;
    }

    public async Task DeleteUser(int id)
    {
        var res = await httpClient.DeleteAsync($"users/{id}");
        if (!res.IsSuccessStatusCode) throw new Exception("Kunne ikke slette bruger. " + res.StatusCode);
    }
}