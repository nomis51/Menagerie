using System.Text;
using System.Threading.Tasks.Dataflow;
using Menagerie.Data.Providers;
using Menagerie.Shared.Abstractions;
using Menagerie.Shared.Extensions;
using Menagerie.Shared.Models.Poe;
using Menagerie.Shared.Models.Poe.BulkTrade;
using Menagerie.Shared.Models.Poe.Stash;
using Menagerie.Shared.Models.Poe.Trade;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;

namespace Menagerie.Data.Services;

public class PoeApiService : IService
{
    #region Constants

    private const string ItemsDataFilePath = "./data/items.json";
    private const string ItemsCategoryFilePath = "./data/item-categories.json";

    private const int FetchedPricesExpiration = 1 * 60;
    private const int RefreshStashTabsThrottleTimeout = 10;
    private const int FetchStashTabsThrottleTimeout = 30 * 1000;
    private const string LeaguesUrl = "leagues?compact=1";
    private const string BulkTradeUrl = "api/trade/exchange/{0}";

    private const string CharactersUrl =
        "character-window/get-stash-items?league={0}&tabs={1}&tabIndex={2}&accountName={3}";

    private const string PoeTradeApiUrl = "api/trade/search/{0}";
    private const string PoeTradeApiFetchUrl = "api/trade/fetch/{0}?query={1}";

    private static readonly SemaphoreSlim FetchStashTabsLock = new(1, 1);
    private static readonly object RefreshStashTabsLock = new();
    private static readonly SemaphoreSlim FetchItemsLock = new(2, 2);
    private static readonly object LastFetchItemsTimeLock = new();

    #endregion

    #region Members

    private readonly Dictionary<string, Tuple<DateTime, string>> _itemNameToApiPrice = new();
    private DateTime _lastFetchItemsTime = DateTime.Now.AddDays(-365);
    private readonly List<string> _craftableItemTypes = new();
    private readonly List<ItemData> _itemsData = new();
    private Dictionary<string, string> _itemBaseToCategory = new();

    #endregion

    #region Public methods

    public void Initialize()
    {
        LoadItemsData();
    }

    public Task Start()
    {
        AutoFetchStashTabs();
        return Task.CompletedTask;
    }

    public async Task<BulkTradeResponse?> FetchBulkTrade(BulkTradeRequest request)
    {
        try
        {
            var json = JsonConvert.SerializeObject(request,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
            var settings = AppDataService.Instance.GetSettings();
            var response = await HttpProvider.PoeWebsite!.Client.PostAsync(
                string.Format(BulkTradeUrl, settings.General.League),
                new StringContent(json, Encoding.UTF8, "application/json")
            ).ConfigureAwait(false);
            var result = await HttpProvider.ReadResponse<BulkTradeResponse>(response);

            if (result is null) throw new Exception("Unable to parse response");
            return result;
        }
        catch (Exception e)
        {
            Log.Error("Unable to fetch leagues {}", e.Message);
        }

        return null;
    }

    public string GetItemImageLink(string itemName)
    {
        var possibleLink = string.Empty;

        foreach (var itemData in _itemsData)
        {
            if (itemName.StartsWith(itemData.Name)) return itemData.Icon;
            if (itemName.EndsWith(itemData.Name))
            {
                possibleLink = itemData.Icon;
            }
        }

        return possibleLink;
    }

    public Tuple<int, int> GetItemSize(string name)
    {
        var itemData = _itemsData.Find(i => name.EndsWith(i.Name));
        if (itemData is null) return new Tuple<int, int>(1, 1);

        if (itemData.Unique is not null)
        {
            if (_itemBaseToCategory.ContainsKey(itemData.Unique.Base))
            {
                itemData.Craftable = new Craftable
                {
                    Category = _itemBaseToCategory[itemData.Unique.Base]
                };
            }
        }

        if (itemData.Craftable is not null)
        {
            switch (itemData.Craftable.Category)
            {
                case "Staff":
                case "Warstaff":
                case "Two-Handed Axe":
                case "Two-Handed Mace":
                case "Two-Handed Sword":
                    return new Tuple<int, int>(2, 4);

                case "Body Armour":
                case "Bow":
                case "Shield":
                case "Quiver":
                    return new Tuple<int, int>(2, 3);

                case "Helmet":
                case "Boots":
                case "Gloves":
                case "Claw":
                case "Scepter":
                    return new Tuple<int, int>(2, 2);

                case "Wand":
                case "Rune Dagger":
                case "Dagger":
                case "One-Handed Mace":
                    return new Tuple<int, int>(1, 3);

                case "Belt":
                    return new Tuple<int, int>(2, 1);

                case "One-Handed Sword":
                    return new Tuple<int, int>(1, 4);

                default:
                    return new Tuple<int, int>(1, 1);
            }
        }

        return new Tuple<int, int>(1, 1);
    }

    public async Task<List<string>> FetchLeagues()
    {
        try
        {
            var response = await HttpProvider.AnonymousPoeApi.Client.GetAsync(LeaguesUrl).ConfigureAwait(false);
            var result = await HttpProvider.ReadResponse<List<Dictionary<string, string>>>(response);

            if (result is null) throw new Exception("Unable to parse response");

            return (from r in result where !r["id"].Contains("SSF") select r["id"]).ToList();
        }
        catch (Exception e)
        {
            Log.Error("Unable to fetch leagues {}", e.Message);
        }

        return new List<string>();
    }

    public async Task<string> VerifyPrice(string itemName, string priceStr)
    {
        if (string.IsNullOrEmpty(itemName) || string.IsNullOrEmpty(priceStr)) return string.Empty;
        if (_itemNameToApiPrice.ContainsKey(itemName) && (DateTime.Now - _itemNameToApiPrice[itemName].Item1).TotalSeconds <= FetchedPricesExpiration)
            return _itemNameToApiPrice[itemName].Item2;

        _itemNameToApiPrice.Remove(itemName);

        var craftableType = _craftableItemTypes.Find(itemName.EndsWith);

        var prices = await SearchItemPrices(itemName, craftableType);
        var pricesList = prices.ToList();
        if (!pricesList.Any()) return string.Empty;

        var result = pricesList.FirstOrDefault(p => p == priceStr) ?? string.Empty;
        if (string.IsNullOrEmpty(result))
        {
            result = pricesList.FirstOrDefault() ?? string.Empty;
        }

        if (!_itemNameToApiPrice.ContainsKey(itemName) && !string.IsNullOrEmpty(result))
        {
            _itemNameToApiPrice.Add(itemName, new Tuple<DateTime, string>(DateTime.Now, result));
        }

        return result;
    }

    #endregion

    #region Private methods

    private async Task<IEnumerable<string>> SearchItemPrices(string name, string? craftableType)
    {
        await FetchItemsLock.WaitAsync();

        if ((DateTime.Now - _lastFetchItemsTime).TotalSeconds <= 10)
        {
            Thread.Sleep(new Random().Next(1000, 3000));
        }

        if (HttpProvider.PoeWebsite is null) return new List<string>();
        var settings = AppDataService.Instance.GetSettings();

        try
        {
            var json = JsonConvert.SerializeObject(new PricingRequest(string.IsNullOrEmpty(craftableType) ? name : craftableType, settings.General.AccountName),
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
            var response = await HttpProvider.PoeWebsite.Client.PostAsync(
                string.Format(PoeTradeApiUrl, settings.General.League),
                new StringContent(json, Encoding.UTF8, "application/json")
            );

            if (!response.IsSuccessStatusCode) return new List<string>();

            var pricingResponse = await HttpProvider.ReadResponse<PricingResponse>(response);
            if (pricingResponse is null) return new List<string>();

            return await FetchItems(pricingResponse);
        }
        finally
        {
            lock (LastFetchItemsTimeLock)
            {
                _lastFetchItemsTime = DateTime.Now;
            }

            FetchItemsLock.Release();
        }
    }

    private async Task<IEnumerable<string>> FetchItems(PricingResponse pricingResponse)
    {
        if (HttpProvider.PoeWebsite is null) return new List<string>();

        try
        {
            var response = await HttpProvider.PoeWebsite.Client.GetAsync(string.Format(PoeTradeApiFetchUrl, string.Join(",", pricingResponse.Result), pricingResponse.Id));

            if (!response.IsSuccessStatusCode) return new List<string>();

            var result = await HttpProvider.ReadResponse<FetchItemsResponse>(response);
            if (result is null) return new List<string>();

            return result.Result
                .Select(r => $"{r.Listing.Price.Amount} {r.Listing.Price.Currency}");
        }
        catch (Exception)
        {
            //ignored
        }

        return new List<string>();
    }

    private void AutoFetchStashTabs()
    {
        Task.Run(async () =>
        {
            Thread.Sleep(3000);

            while (true)
            {
                var settings = AppDataService.Instance.GetSettings();
                if (settings.ChaosRecipe.Enabled && !string.IsNullOrEmpty(settings.General.Poesessid) && !string.IsNullOrEmpty(settings.General.AccountName))
                {
                    await FetchStashTab(settings.ChaosRecipe.StashTabIndex);
                }

                Thread.Sleep(settings.ChaosRecipe.RefreshRate * 60 * 1000);
            }
        });
    }

    private async Task<StashTabResponse> FetchStashTab(int index, bool fetchTabs = false)
    {
        try
        {
            if (HttpProvider.PoeWebsite is null) return null;

            var settings = AppDataService.Instance.GetSettings();
            if (string.IsNullOrEmpty(settings.General.Poesessid) || string.IsNullOrEmpty(settings.General.AccountName)) return null;

            var response = await HttpProvider.PoeWebsite.Client.GetAsync(string.Format(CharactersUrl,
                settings.General.League, fetchTabs ? 1 : 0, index, settings.General.AccountName));
            var result = await HttpProvider.ReadResponse<StashTabResponse>(response);
            if (result is null) throw new Exception("Unable to parse stash tab response");

            return result;
        }
        catch (Exception e)
        {
            Log.Error("Unable to fetch stash tab {index} {message}", index, e.Message);
        }

        return null;
    }

    private void LoadItemsData()
    {
        var data = File.ReadAllText(ItemsDataFilePath);
        if (!string.IsNullOrEmpty(data))
        {
            try
            {
                var itemsData = JsonConvert.DeserializeObject<List<ItemData>>(data);
                if (itemsData is null) throw new NullReferenceException();

                foreach (var itemData in itemsData)
                {
                    if (itemData.Craftable is not null && !string.IsNullOrEmpty(itemData.Craftable.Category))
                    {
                        _craftableItemTypes.Add(itemData.Name);
                    }

                    _itemsData.Add(itemData);
                }
            }
            catch (Exception)
            {
                Log.Warning("Unable to parse items data");
            }
        }

        var data2 = File.ReadAllText(ItemsCategoryFilePath);
        if (string.IsNullOrEmpty(data2)) return;

        try
        {
            _itemBaseToCategory = JsonConvert.DeserializeObject<Dictionary<string, string>>(data2);
        }
        catch (Exception e)
        {
            Log.Warning("Unable to parse items categories: {Message}", e.Message);
        }
    }

    #endregion
}