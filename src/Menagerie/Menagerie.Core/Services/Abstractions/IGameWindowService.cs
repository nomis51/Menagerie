namespace Menagerie.Core.Services.Abstractions;

public interface IGameWindowService : IService
{
    void SetProcessId(int processId);
    bool FocusGameWindow();
    bool IsGameWindowFocused(int delay = 50);
    void AutoHideOverlay();
}