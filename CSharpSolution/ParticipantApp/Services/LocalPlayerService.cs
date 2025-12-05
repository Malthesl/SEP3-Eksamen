using Microsoft.JSInterop;

namespace ParticipantApp.Services;

public class LocalPlayerService(IJSRuntime js) : IPlayerService
{
    public async Task<string?> GetPlayerIdAsync()
    {
        return await js.InvokeAsync<string>("getItem", "playerId");
    }

    public async Task<string?> GetGameIdAsync()
    {
        return await js.InvokeAsync<string>("getItem", "gameId");
    }

    public async Task Set(string playerId, string gameId)
    {
        await js.InvokeVoidAsync("setItem", "playerId", playerId);
        await js.InvokeVoidAsync("setItem", "gameId", gameId);
    }

    public async Task Clear()
    {
        await js.InvokeVoidAsync("clear");
    }
}