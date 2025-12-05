namespace ParticipantApp.Services;

public interface IPlayerService
{
    public Task<string?> GetPlayerIdAsync(); 
    public Task<string?> GetGameIdAsync(); 
    public Task Set(string playerId, string gameId);
    public Task Clear();
}