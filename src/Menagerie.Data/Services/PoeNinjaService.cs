using Menagerie.Data.Providers;
using Menagerie.Shared.Abstractions;
using Menagerie.Shared.Helpers;
using Menagerie.Shared.Models.PoeNinja;
using Serilog;

namespace Menagerie.Data.Services;

public class PoeNinjaService : IService
{
    #region Constants

    private const string CurrencyUrl = "api/data/currencyoverview?league={0}&type=Currency&language=en";

    #endregion

    #region Members

    private readonly Dictionary<string, double> _currencyToChaos = new();

    #endregion

    #region Constructors

    public PoeNinjaService()
    {
    }

    #endregion

    #region Public methods

    public void Initialize()
    {
    }

    public Task Start()
    {
        AutoFetchCurrencies();
        return Task.CompletedTask;
    }

    public Tuple<string, int, double> CalculatePriceConversions(double value, string currency)
    {
        var currencyRealName = CurrencyHelper.GetRealName(currency);
        var isChaos = currencyRealName == "Chaos Orb";
        if (!_currencyToChaos.ContainsKey(currencyRealName) || (isChaos && _currencyToChaos.ContainsKey("Exalted Orb") && value < _currencyToChaos["Exalted Orb"]))
            return new Tuple<string, int, double>(string.Empty, 0, 0);

        var chaosValue = isChaos ? value : _currencyToChaos[currencyRealName] * value;
        var exaltValue = !_currencyToChaos.ContainsKey("Exalted Orb") ? 0 : chaosValue / _currencyToChaos["Exalted Orb"];
        var intExaltValue = (int)exaltValue;
        var exaltFloatingChaosValue = !_currencyToChaos.ContainsKey("Exalted Orb") ? 0 : (exaltValue - intExaltValue) * _currencyToChaos["Exalted Orb"];

        return new Tuple<string, int, double>(
            string.Join(" | ", new[]
            {
                currencyRealName == "Chaos Orb" ? string.Empty : $"{Math.Round(chaosValue)} chaos",
                currencyRealName == "Exalted Orb"
                    ? string.Empty
                    : $"{Math.Round(exaltValue, 1)} exalted",
                intExaltValue == 0
                    ? string.Empty
                    : $"{intExaltValue} exalted + {Math.Round(exaltFloatingChaosValue)} chaos"
            }.Where(s => !string.IsNullOrEmpty(s))),
            (int)Math.Round(chaosValue, MidpointRounding.ToEven),
            exaltValue
        );
    }

    public double CalculateChaosValue(double value, string currency)
    {
        var currencyRealName = CurrencyHelper.GetRealName(currency);
        if (currencyRealName == "Chaos Orb") return value;
        if (!_currencyToChaos.ContainsKey(currencyRealName)) return 0;
        return Math.Round(_currencyToChaos[currencyRealName] * value, 1);
    }

    public double CalculateExaltedValue(double chaosValue)
    {
        if (!_currencyToChaos.ContainsKey("Exalted Orb")) return 0;
        var exaltedValue = _currencyToChaos["Exalted Orb"];
        return Math.Round(chaosValue / exaltedValue, 1);
    }

    #endregion

    #region Private methods

    private void AutoFetchCurrencies()
    {
        Task.Run(async () =>
        {
            while (true)
            {
                await FetchCurrencies();
                var settings = AppDataService.Instance.GetSettings();
                Thread.Sleep(settings.App.PoeNinjaRefreshRate * 60 * 1000);
            }
        });
    }


    private async Task FetchCurrencies()
    {
        try
        {
            var settings = AppDataService.Instance.GetSettings();
            var response =
                await HttpProvider.PoeNinja.Client.GetAsync(string.Format(CurrencyUrl, settings.General.League));
            var result = await HttpProvider.ReadResponse<PoeNinjaResult<PoeNinjaCurrency>>(response);
            if (result is null) return;

            foreach (var line in result.Lines)
            {
                _currencyToChaos[line.CurrencyTypeName] = line.ChaosEquivalent;
            }
        }
        catch (Exception e)
        {
            Log.Error("Error fetching poe.ninja currencies {}", e.Message);
        }
    }

    #endregion
}