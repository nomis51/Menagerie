using AutoMapper;
using Menagerie.Application.DTOs;
using Menagerie.Application.DTOs.Settings;
using Menagerie.Shared.Helpers;
using Menagerie.Shared.Models;
using Menagerie.Shared.Models.Chat;
using Menagerie.Shared.Models.Poe;
using Menagerie.Shared.Models.Poe.Stash;
using Menagerie.Shared.Models.Setting;
using Menagerie.Shared.Models.Trading;
using Serilog;

namespace Menagerie.Application;

public class AppMapper
{
    #region Singleton

    private static readonly object LockInstance = new object();
    private static AppMapper _instance;

    public static AppMapper Instance
    {
        get
        {
            lock (LockInstance)
            {
                _instance ??= new AppMapper();
            }

            return _instance;
        }
    }

    #endregion

    #region Members

    private readonly IMapper _mapper;

    #endregion

    #region Constructors

    private AppMapper()
    {
        var configuration = new MapperConfiguration(config =>
        {
            config.CreateMap<IncomingOffer, IncomingOfferDto>();
            config.CreateMap<OutgoingOffer, OutgoingOfferDto>();
            TwoWayMap<Settings, SettingsDto>(config);
            TwoWayMap<GeneralSettings, GeneralSettingsDto>(config);
            TwoWayMap<IncomingTradesSettings, IncomingTradesSettingsDto>(config);
            TwoWayMap<OutgoingTradesSettings, OutgoingTradesSetingsDto>(config);
            TwoWayMap<ChaosRecipeSettings, ChaosRecipeSettingsDto>(config);
            config.CreateMap<ChatMessage, ChatMessageDto>();
            config.CreateMap<TradeStats, TradeStatsDto>();
            config.CreateMap<StashTab, StashTabDto>();
            config.CreateMap<Item, ItemDto>();
            config.CreateMap<TabColor, TabColorDto>();
            config.CreateMap<Requirement, RequirementDto>();
            config.CreateMap<Property, PropertyDto>();
            config.CreateMap<LogbookModifier, LogbookModifierDto>();
            config.CreateMap<LogbookFaction, LogbookFactionDto>();
            config.CreateMap<Socket, SocketDto>();
            config.CreateMap<StashTabGridSettings, StashTabGridSettingsDto>();
        });

        _mapper = new Mapper(configuration);
    }

    #endregion

    #region Private methods

    private static void TwoWayMap<T1, T2>(IProfileExpression config)
    {
        config.CreateMap<T1, T2>();
        config.CreateMap<T2, T1>();
    }

    #endregion

    #region Public methods

    public TDestination Map<TDestination>(object obj) where TDestination : class
    {
        if (typeof(TDestination) == typeof(List<ChaosRecipeItemDto>))
            return (TDestination)MapChaosRecipe(obj as ChaosRecipe);

        try
        {
            return _mapper.Map<TDestination>(obj);
        }
        catch (Exception e)
        {
            Log.Error("Unable to map {} to {}: {}. {}", obj.GetType().ToString(), typeof(TDestination).ToString(),
                e.Message, e.StackTrace);
            throw;
        }
    }

    #endregion

    #region Private methods

    private static IEnumerable<ChaosRecipeItemDto> MapChaosRecipe(ChaosRecipe? input)
    {
        return input is null
            ? new List<ChaosRecipeItemDto>()
            : new List<ChaosRecipeItemDto>
            {
                new("Helmets", input.NbHelmets, ItemHelper.GetItemCategoryImageLink("helmet")),
                new("Boots", input.NbBoots, ItemHelper.GetItemCategoryImageLink("boots")),
                new("Gloves", input.NbGloves, ItemHelper.GetItemCategoryImageLink("gloves")),
                new("Belts", input.NbBelts, ItemHelper.GetItemCategoryImageLink("belt")),
                new("Body Armours", input.NbBodyArmours, ItemHelper.GetItemCategoryImageLink("body armour")),
                new("Rings", input.NbRingSets, ItemHelper.GetItemCategoryImageLink("ring")),
                new("Amulets", input.NbAmulets, ItemHelper.GetItemCategoryImageLink("amulet")),
                new("Weapons", input.NbWeaponSets, ItemHelper.GetItemCategoryImageLink("weapon")),
                new("Sets", input.NbSets),
            };
    }

    #endregion
}