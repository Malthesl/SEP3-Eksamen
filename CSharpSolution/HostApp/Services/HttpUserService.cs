using ApiContracts;
using static HostApp.Utils.Utils;

namespace HostApp.Services;

public class HttpUserService(HttpClient httpClient) : IUserService
{
    public async Task<UserDTO> CreateUser(string username, string password)
    {
        var response = await httpClient.PostAsJsonAsync("users", new { Username = username, Password = password });
        await CheckResponse(response);
        
        return (await response.Content.ReadFromJsonAsync<UserDTO>())!;
    }

    public async Task<UserDTO> GetUser(int id)
    {
        var response = await httpClient.GetAsync($"users/{id}");
        await CheckResponse(response);
        
        return (await response.Content.ReadFromJsonAsync<UserDTO>())!;
    }
    public async Task<UserDTO> UpdateUsername(int userId, string username)
    {
        var res = await httpClient.PostAsJsonAsync($"/users/{userId}", new { Username = username});
        await CheckResponse(res);
        return (await res.Content.ReadFromJsonAsync<UserDTO>())!;
    }

    public async Task<UserDTO> UpdatePassword(int userId, string newPassword)
    {
        var res = await  httpClient.PostAsJsonAsync($"/users/{userId}", new { Password = newPassword});
        await CheckResponse(res);
        return (await res.Content.ReadFromJsonAsync<UserDTO>())!;
    }

    public async Task<UserDTO> UpdateUser(UserDTO user)
    {
        var response = await httpClient.PostAsJsonAsync("users", user);
        await CheckResponse(response);
        
        return (await response.Content.ReadFromJsonAsync<UserDTO>())!;
    }

    public async Task DeleteUser(int id)
    {
        var res = await httpClient.DeleteAsync($"users/{id}");
        await CheckResponse(res);
    }
}