using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Menagerie.Application.Services;
using NuGet;
using ReactiveUI;

namespace Menagerie.ViewModels;

public class TranslationToolViewModel : ReactiveObject
{
    #region Members

    private string _message = string.Empty;
    private string _translatedMessage = string.Empty;
    private bool _isSendButtonActive;

    #endregion

    #region Props

    public ReadOnlyObservableCollection<string> Languages { get; }


    public string Message
    {
        get => _message;
        set
        {
            this.RaiseAndSetIfChanged(ref _message, value.Trim());
            TranslateMessage();
        }
    }

    public string TranslatedMessage
    {
        get => _translatedMessage;
        private set
        {
            this.RaiseAndSetIfChanged(ref _translatedMessage, value);
            IsSendButtonActive = !string.IsNullOrEmpty(_translatedMessage);
        }
    }

    public string SourceLanguage { get; set; } = "Automatic";
    public string TargetLanguage { get; set; }

    public bool IsSendButtonActive
    {
        get => _isSendButtonActive;
        set => this.RaiseAndSetIfChanged(ref _isSendButtonActive, value);
    }

    #endregion

    #region Constructors

    public TranslationToolViewModel()
    {
        Languages = new ReadOnlyObservableCollection<string>(new ObservableCollection<string>(AppService.Instance.GetLanguages()));
    }

    #endregion

    #region Public methods

    public void TranslateMessage()
    {
        if (string.IsNullOrEmpty(TargetLanguage) || string.IsNullOrEmpty(SourceLanguage)) return;

        Task.Run(() =>
        {
            var isWhisper = _message.StartsWith("@");
            var hasChatTag = _message.StartsWith("#") || _message.StartsWith("%") || _message.StartsWith("$") || isWhisper;
            var spaceIndex = _message.IndexOf(" ", StringComparison.Ordinal);

            var actualMessage = isWhisper ? _message[(spaceIndex + 1)..] : hasChatTag ? _message[1..] : _message;

            var translatedMessage = AppService.Instance.Translate(actualMessage, SourceLanguage, TargetLanguage).Result ?? string.Empty;

            System.Windows.Application.Current.Dispatcher.Invoke(delegate
            {
                TranslatedMessage = isWhisper ? $"@{_message[1..spaceIndex]} {translatedMessage}" : hasChatTag ? $"{_message[0]}{translatedMessage}" : translatedMessage;
            });
        });
    }

    public void SendMessage()
    {
        if (string.IsNullOrEmpty(TranslatedMessage)) return;

        AppService.Instance.SendChatMessage(TranslatedMessage);
        TranslatedMessage = string.Empty;
        Message = string.Empty;
    }

    #endregion
}