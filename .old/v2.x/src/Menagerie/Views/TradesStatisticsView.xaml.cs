using System;
using System.Drawing;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using Menagerie.ViewModels;
using ReactiveUI;

namespace Menagerie.Views;

public partial class TradesStatisticsView
{
    public delegate void CloseEvent();

    public event CloseEvent OnClose;

    public TradesStatisticsView()
    {
        InitializeComponent();

        ViewModel = new TradesStatisticsViewModel();

        this.WhenActivated(disposableRegistration =>
        {
            this.OneWayBind(ViewModel,
                    x => x.CurrencySeriesCollection,
                    x => x.PieChartCurrency.Series)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.ItemTypesSeriesCollection,
                    x => x.PieChartItemTypes.Series)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.TradesSeriesCollection,
                    x => x.CartesianChartTrades.Series)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.TradesLabels,
                    x => x.TradesAxisX.Labels)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.ChaosValuesSeriesCollection,
                    x => x.CartesianChartChaosValues.Series)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.ChaosValuesLabels,
                    x => x.ChaosValuesAxisX.Labels)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.NbTradesTodayStr,
                    x => x.TextBlockNbTrades.Text)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.ChaosValueTodayStr,
                    x => x.TextBlockChaosValueToday.Text)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.ExaltedValueTodayStr,
                    x => x.TextBlockExaltedValueToday.Text)
                .DisposeWith(disposableRegistration);

            var darkBackground = FindResource("DarkBackground") as SolidColorBrush;
            CartesianChartTrades.DataTooltip.Background = darkBackground;
            CartesianChartChaosValues.DataTooltip.Background = darkBackground;
            PieChartCurrency.DataTooltip.Background = darkBackground;
            PieChartItemTypes.DataTooltip.Background = darkBackground;
        });
    }

    private void ButtonTopBarClose_OnClick(object sender, RoutedEventArgs e)
    {
        OnClose?.Invoke();
    }

    public void ReloadStats()
    {
        ViewModel?.LoadStats();
    }
}