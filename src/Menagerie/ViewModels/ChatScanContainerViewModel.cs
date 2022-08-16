using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using DynamicData;
using Menagerie.Application.DTOs;
using Menagerie.Application.Events;
using ReactiveUI;

namespace Menagerie.ViewModels;

public class ChatScanContainerViewModel : ReactiveObject
{
    #region Members

    private readonly SourceList<ChatScanMessageViewModel> _chatScanMessages = new();

    #endregion

    #region Props

    public ReadOnlyObservableCollection<ChatScanMessageViewModel> ChatScanMessages;

    #endregion

    #region Constructors

    public ChatScanContainerViewModel()
    {
        _chatScanMessages
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out ChatScanMessages)
            .Subscribe();

        AppEvents.OnChatMessageFound += AppEvents_OnChatMessageFound;
    }

    #endregion

    #region Private methods

    private void AppEvents_OnChatMessageFound(ChatMessageDto chatMessage)
    {
        AddChatScanMessage(chatMessage);
    }

    private void ChatScanMessageViewModel_OnMessageRemoved(ChatScanMessageViewModel vm)
    {
        RemoveChatScanMessage(vm);
    }

    private void RemoveChatScanMessage(ChatScanMessageViewModel vm)
    {
        vm.OnMessageRemoved -= ChatScanMessageViewModel_OnMessageRemoved;
        _chatScanMessages.Remove(vm);
    }

    private void AddChatScanMessage(ChatMessageDto chatMessage)
    {
        var vm = new ChatScanMessageViewModel(chatMessage);
        vm.OnMessageRemoved += ChatScanMessageViewModel_OnMessageRemoved;
        _chatScanMessages.Add(vm);
    }

    #endregion
}