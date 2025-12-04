using ApiContracts;

namespace BlazorApp.Services;

public interface IUserService
{
    public Task<UserDTO> CreateUser(string username, string password);
}