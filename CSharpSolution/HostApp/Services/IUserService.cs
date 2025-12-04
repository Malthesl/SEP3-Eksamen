using ApiContracts;

namespace HostApp.Services;

public interface IUserService
{
    public Task<UserDTO> CreateUser(string username, string password);
}