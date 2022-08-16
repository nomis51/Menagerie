using Ganss.Text;
using Menagerie.Shared.Abstractions;
using Menagerie.Shared.Models.Chat;

namespace Menagerie.Data.Services;

public class ChatScanService : IService
{
    #region Public methods

    public void Initialize()
    {
    }

    public Task Start()
    {
        return Task.CompletedTask;
    }

    public void Analyse(ChatMessage chatMessage)
    {
        var settings = AppDataService.Instance.GetSettings();
        var matches = chatMessage.Message.Contains(CharComparer.OrdinalIgnoreCase, settings.ChatScan.Words);
        if (matches is null || !matches.Any()) return;

        AppDataService.Instance.ChatMessageFound(chatMessage);
    }

    #endregion
}