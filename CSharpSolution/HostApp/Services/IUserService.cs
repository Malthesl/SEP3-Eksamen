using ApiContracts;

namespace HostApp.Services;

public interface IUserService
{
    public Task<UserDTO> CreateUser(string username, string password);
    public Task<UserDTO> GetUser(int id);
    public Task<UserDTO> UpdateUsername (int userId, string newUsername);
    public Task<UserDTO> UpdatePassword(int userId, string newPassword);
    public Task DeleteUser(int id);
}