using AutoMapper;
using Menagerie.Models;
using CoreModels = Menagerie.Core.Models;

namespace Menagerie
{
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

        private IMapper Mapper { get; }

        #endregion

        #region Constructors

        private AppMapper()
        {
            var configuration = new MapperConfiguration(config =>
            {
                config.CreateMap<Core.Models.Trades.Offer, Offer>();
                config.CreateMap<Offer, Core.Models.Trades.Offer>();
                config.CreateMap<CoreModels.PriceCheckResult, PriceCheckResult>();
                config.CreateMap<CoreModels.PricingResult, PricingResult>();
                config.CreateMap<CoreModels.PoeApi.Stash.ChaosRecipeResult, ChaosRecipeResult>();
                config.CreateMap<CoreModels.Config, Config>();
                config.CreateMap<Config, CoreModels.Config>();
                config.CreateMap<CoreModels.ItemsScan.MapModifier, MapModifier>();
            });

            Mapper = new Mapper(configuration);
        }

        #endregion

        #region Public methods

        public TTo Map<TFrom, TTo>(TFrom obj)
        {
            return Mapper.Map<TFrom, TTo>(obj);
        }

        #endregion
    }
}