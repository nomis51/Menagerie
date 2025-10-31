namespace Menagerie.Core.Services.Abstractions;

public interface IGameService
{
    EventHandler<int>? GameProcessChanged { get; set; }
    int GameProcessId { get; }
    Task InitializeAsync();
    string? GetGameClientLogFilePathAsync();
    Task<bool> FocusOverlayAsync();
    Task<bool> FocusGameAsync();
}