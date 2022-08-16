using System;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Menagerie.Helpers;
using Menagerie.ViewModels;
using ReactiveUI;

namespace Menagerie.Views;

public partial class TranslationToolView
{
    #region Events

    public delegate void CloseEvent();

    public event CloseEvent OnClose;

    #endregion

    #region Members

    private readonly TextBoxDebouncer _messageTextBoxDebouncer;

    #endregion

    public TranslationToolView()
    {
        InitializeComponent();

        ViewModel = new TranslationToolViewModel();
        _messageTextBoxDebouncer = new TextBoxDebouncer();
        _messageTextBoxDebouncer.OnIdled += MessageTextBoxDebouncerOnIdled;

        this.WhenActivated(disposableRegistration =>
        {
            this.OneWayBind(ViewModel,
                    x => x.Languages,
                    x => x.ComboBoxSourceLanguage.ItemsSource)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.Languages,
                    x => x.ComboBoxTargetLanguage.ItemsSource)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.Message,
                    x => x.TextBoxMessage.Text)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.TranslatedMessage,
                    x => x.TextBlockTranslatedMessage.Text)
                .DisposeWith(disposableRegistration);

            this.Bind(ViewModel,
                    x => x.SourceLanguage,
                    x => x.ComboBoxSourceLanguage.SelectedValue)
                .DisposeWith(disposableRegistration);

            this.Bind(ViewModel,
                    x => x.TargetLanguage,
                    x => x.ComboBoxTargetLanguage.SelectedValue)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.IsSendButtonActive,
                    x => x.ButtonSend.IsEnabled)
                .DisposeWith(disposableRegistration);
        });
    }

    public void FocusTextBox()
    {
        Keyboard.Focus(TextBoxMessage);
    }

    private void MessageTextBoxDebouncerOnIdled(object? sender, EventArgs e)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(delegate { ViewModel!.Message = TextBoxMessage.Text; });
    }

    private void TextBoxMessage_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        _messageTextBoxDebouncer.TextChanged();
    }

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
    {
        OnClose?.Invoke();
    }

    private void ButtonSend_OnClick(object sender, RoutedEventArgs e)
    {
        ViewModel?.SendMessage();
    }

    private void ButtonTranslate_OnClick(object sender, RoutedEventArgs e)
    {
        ViewModel?.TranslateMessage();
    }
}