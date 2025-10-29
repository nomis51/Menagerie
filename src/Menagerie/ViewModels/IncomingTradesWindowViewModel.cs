using System.Collections.ObjectModel;
using Menagerie.ViewModels.Abstractions;

namespace Menagerie.ViewModels;

public class IncomingTradesWindowViewModel : ViewModelBase
{
    #region Members

    public int TradeTileSize { get; set; }
    public ObservableCollection<int> Trades { get; } = [];

    #endregion

    #region Public methods

    public void RemoveAllTrades()
    {
        Trades.Clear();
        OnPropertyChanged(nameof(Trades));
    }

    #endregion
}