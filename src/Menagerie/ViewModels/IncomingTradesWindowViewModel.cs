using System;
using System.Collections.ObjectModel;
using Menagerie.Core.Models.Trading;
using Menagerie.Core.Services.Abstractions;
using Menagerie.ViewModels.Abstractions;

namespace Menagerie.ViewModels;

public class IncomingTradesWindowViewModel : ViewModelBase, IDisposable
{
    #region Members

    private readonly ITextParsingService _textParsingService;
    private readonly IGameChatService _gameChatService;
    private readonly IAudioService _audioService;

    public int TradeTileSize { get; set; }
    public ObservableCollection<IncomingTradeViewModel> Trades { get; } = [];

    #endregion

    #region Constructors

    public IncomingTradesWindowViewModel(
        ITextParsingService textParsingService,
        IGameChatService gameChatService,
        IAudioService audioService
    )
    {
        _textParsingService = textParsingService;
        _gameChatService = gameChatService;
        _audioService = audioService;
        _textParsingService.NewIncomingTrade += NewIncomingTrade;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _textParsingService.NewIncomingTrade -= NewIncomingTrade;
    }

    #endregion

    #region Events

    private void NewIncomingTrade(object? sender, Trade e)
    {
        Trades.Add(
            new IncomingTradeViewModel(
                TradeTileSize,
                _audioService,
                _gameChatService,
                e
            )
        );
        OnPropertyChanged(nameof(Trades));
    }

    #endregion

    #region Public methods

    public void RemoveAllTrades()
    {
        Trades.Clear();
        OnPropertyChanged(nameof(Trades));
    }

    #endregion
}