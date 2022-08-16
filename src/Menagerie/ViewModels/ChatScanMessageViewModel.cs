using System.Threading;
using System.Threading.Tasks;
using Menagerie.Application.DTOs;
using Menagerie.Application.Services;
using Menagerie.Shared.Models.Chat;
using ReactiveUI;

namespace Menagerie.ViewModels;

public class ChatScanMessageViewModel : ReactiveObject
{
    #region Events

    public delegate void MessageRemovedEvent(ChatScanMessageViewModel viewModel);

    public event MessageRemovedEvent? OnMessageRemoved;

    #endregion

    #region Members

    private ChatMessageDto _chatMessage;

    #endregion

    #region Props

    public ChatMessageDto ChatMessage
    {
        get => _chatMessage;
        set => _chatMessage = value;
    }

    public string PlayerName => _chatMessage.Player;
    public string Time => _chatMessage.Time.ToLocalTime().ToString("hh:mm");
    public string Message => _chatMessage.Message;
    public string Type => $"{_chatMessage.Type} chat:";

    #endregion

    #region Constructors

    public ChatScanMessageViewModel(ChatMessageDto chatMessage)
    {
        _chatMessage = chatMessage;
        SetAutoRemoveAfterDelay();
    }

    #endregion

    #region Public methods

    public void Remove()
    {
        OnMessageRemoved?.Invoke(this);
    }

    #endregion

    #region Private methods

    private void SetAutoRemoveAfterDelay()
    {
        var settings = AppService.Instance.GetSettings();
        if (!settings.ChatScan.AutoRemoveMessage) return;

        _ = Task.Run(() =>
        {
            Thread.Sleep(settings.ChatScan.AutoRemoveMessageDelay * 1000);
            Remove();
        });
    }

    #endregion
}