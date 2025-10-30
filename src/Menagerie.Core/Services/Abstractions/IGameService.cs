namespace Menagerie.Core.Services.Abstractions;

public interface IGameService
{
    EventHandler<int>? GameProcessChanged { get; set; }
    int GameProcessId { get; }
    string? GetGameClientLogFilePathAsync();
    Task<bool> FocusOverlayAsync();
    Task<bool> FocusGameAsync();
}