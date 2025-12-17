namespace HostApp.Services;

public interface IAuthService
{
    Task<string> LoginAndReturnToken(string username, string password);
}