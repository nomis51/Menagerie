using log4net;
using Menagerie.Core.Abstractions;
using System.Collections.Generic;
using Menagerie.Core.Extensions;
using System.Net.Http;
using System;

namespace Menagerie.Core.Services
{
    public class CurrencyService : IService
    {
        #region Constants

        private static readonly ILog Log = LogManager.GetLogger(typeof(CurrencyService));

        private readonly Dictionary<string, string> _currencyToImageLink = new()
        {
            {
                "alt",
                "https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyRerollMagic.png?v=6d9520174f6643e502da336e76b730d3"
            },
            {
                "fuse",
                "https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyRerollSocketLinks.png?v=0ad7134a62e5c45e4f8bc8a44b95540f"
            },
            {
                "alch",
                "https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyUpgradeToRare.png?v=89c110be97333995522c7b2c29cae728"
            },
            {
                "chaos",
                "https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyRerollRare.png?v=c60aa876dd6bab31174df91b1da1b4f9"
            },
            {
                "gcp",
                "https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyGemQuality.png?v=f11792b6dbd2f5f869351151bc3a4539"
            },
            {
                "exalted",
                "https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyAddModToRare.png?v=1745ebafbd533b6f91bccf588ab5efc5"
            },
            {
                "chrome",
                "https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyRerollSocketColours.png?v=9d377f2cf04a16a39aac7b14abc9d7c3"
            },
            {
                "jewellers",
                "https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyRerollSocketNumbers.png?v=2946b0825af70f796b8f15051d75164d"
            },
            {
                "chance",
                "https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyUpgradeRandomly.png?v=e4049939b9cd61291562f94364ee0f00"
            },
            {
                "chisel",
                "https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyMapQuality.png?v=f46e0a1af7223e2d4cae52bc3f9f7a1f"
            },
            {
                "vaal",
                "https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyVaal.png?v=64114709d67069cd665f8f1a918cd12a"
            },
            {
                "blessed",
                "https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyImplicitMod.png?v=472eeef04846d8a25d65b3d4f9ceecc8"
            },
            {
                "p",
                "https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyCoin.png?v=b971d7d9ea1ad32f16cce8ee99c897cf"
            },
            {
                "mirror",
                "https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyDuplicate.png?v=6fd68c1a5c4292c05b97770e83aa22bc"
            },
            {
                "transmute",
                "https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyUpgradeToMagic.png?v=333b8b5e28b73c62972fc66e7634c5c8"
            },
            {
                "silver",
                "https://web.poecdn.com/image/Art/2DItems/Currency/SilverObol.png?v=93c1b204ec2736a2fe5aabbb99510bcf"
            },
            {
                "ancient",
                "https://web.poecdn.com/image/Art/2DItems/Currency/AncientOrb.png?v=3edb14b53b9b05e176124814aba86f94"
            },
            {
                "bauble",
                "https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyFlaskQuality.png?v=ca8bd0dd43d2adf8b021578a398eb9de"
            },
            {
                "scouring",
                "https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyConvertToNormal.png?v=15e3ef97f04a39ae284359309697ef7d"
            },
            {
                "regret",
                "https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyPassiveSkillRefund.png?v=1de687952ce56385b74ac450f97fcc33"
            },
            {
                "sextant",
                "https://web.poecdn.com/image/Art/2DItems/Currency/AtlasRadiusTier1.png?v=7c21fc06120d910018893083227406df"
            },
            {
                "prime sextant",
                "https://web.poecdn.com/image/Art/2DItems/Currency/AtlasRadiusTier2.png?v=043e5f4d8c52af6b302e30afaffa5eba"
            },
            {
                "awakened sextant",
                "https://web.poecdn.com/image/Art/2DItems/Currency/AtlasRadiusTier3.png?v=d69377f526ff51c408461b0cee4d4ade"
            },
            {
                "augment",
                "https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyAddModToMagic.png?v=97e63b85807f2419f4208482fd0b4859"
            },
            {
                "annul",
                "https://web.poecdn.com/image/Art/2DItems/Currency/AnnullOrb.png?v=f9a0f8b21515c8abf517e9648cfc7455"
            },
            {
                "regal",
                "https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyUpgradeMagicToRare.png?v=1187a8511b47b35815bd75698de1fa2a"
            },
            {
                "scrap",
                "https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyArmourQuality.png?v=251e204e4ec325f75ce8ef75b2dfbeb8"
            },
            {
                "portal",
                "https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyPortal.png?v=728696ea10d4fb1e789039debc5d8c3c"
            },
            {
                "scroll",
                "https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyIdentification.png?v=1b9b38c45be95c59d8900f91b2afd58b"
            },
            {
                "harbinger",
                "https://web.poecdn.com/image/Art/2DItems/Currency/HarbingerOrb.png?v=a61bf3add692a2fe74bb5f39213f4d93"
            },
            {
                "horizon",
                "https://web.poecdn.com/image/Art/2DItems/Currency/HorizonOrb.png?v=f3b3343dc61c60e667003bbdbbdb2374"
            }
        };

        #endregion

        #region Members

        #endregion

        #region Constructors

        public CurrencyService()
        {
            Log.Trace("Initializing CurrencyService");
        }

        #endregion

        #region Private methods

        private static string GetImage(string link)
        {
            using var handler = new HttpClientHandler();
            using var client = new HttpClient(handler);
            var bytes = client.GetByteArrayAsync(link).Result;
            return Convert.ToBase64String(bytes);
        }

        #endregion

        #region Public Methods

        public static double GetChaosValue(string currencyName)
        {
            var currency = GetRealName(currencyName);
            return AppService.Instance.GetChaosValueOfCurrency(currency);
        }

        public string GetCurrencyImageLink(string currencyName)
        {
            Log.Trace($"Getting currency image link {currencyName}");

            var norm = NormalizeCurrency(currencyName);

            if (!_currencyToImageLink.ContainsKey(norm)) return string.Empty;

            var link = _currencyToImageLink[norm];

            return link;

            //var dbImage = AppService.Instance.GetImage(link);

            //if (dbImage != null) return dbImage.Base64;
            //dbImage = new Models.AppImage()
            //{
            //    Link = link,
            //    Base64 = GetImage(link)
            //};

            //AppService.Instance.SaveImage(dbImage);

            //return dbImage.Base64;
        }

        public static string GetRealName(string text)
        {
            Log.Trace($"Getting real currency name {text}");
            return text switch
            {
                "chaos" => "Chaos Orb",
                "alt" => "Orb of Alteration",
                "alch" => "Orb of Alchemy",
                "gcp" => "Gemcutter's Prism",
                "exalted" => "Exalted Orb",
                "chrome" => "Chromatic Orb",
                "jewellers" => "Jeweller's Orb",
                "chance" => "Orb of Chance",
                "chisel" => "Cartographer's Chisel",
                "vaal" => "Vaal Orb",
                "blessed" => "Blessed Orb",
                "p" => "Perandus Coin",
                "mirror" => "Mirror of Kalandra",
                "transmute" => "Orb of Transmutation",
                "silver" => "Silver Coin",
                "fuse" => "Orb of Fusing",
                "ancient" => "Ancient Orb",
                "bauble" => "Glassblower's Bauble",
                "souring" => "Orb of Scouring",
                "regret" => "Orb of Regret",
                "augment" => "Orb of Augmentation",
                "sextant" => "Simple Sextant",
                "prime sextant" => "Prime Sextant",
                "awakened sextant" => "Awakened Sextant",
                "annul" => "Orb of Annulment",
                "regal" => "Regal Orb",
                "scrap" => "Armourer's Scrap",
                "portal" => "Portal Scroll",
                "scroll" => "Scroll of Wisdom",
                "harbinger" => "Harbinger's Orb",
                "horizon" => "Orb of Horizon",
                _ => text
            };
        }

        public static string NormalizeCurrency(string text)
        {
            Log.Trace($"Normalizing currency name {text}");
            return text switch
            {
                "Chaos Orb" => "chaos",
                "Orb of Alteration" => "alt",
                "Orb of Alchemy" => "alch",
                "Gemcutter's Prism" => "gcp",
                "Exalted Orb" => "exalted",
                "Chromatic Orb" => "chrome",
                "Jeweller's Orb" => "jewellers",
                "Orb of Chance" => "chance",
                "Cartographer's Chisel" => "chisel",
                "Vaal Orb" => "vaal",
                "Blessed Orb" => "blessed",
                "Perandus Coin" => "p",
                "Mirror of Kalandra" => "mirror",
                "Orb of Transmutation" => "transmute",
                "Silver Coin" => "silver",
                "Ancient Orb" => "ancient",
                "Glassblower's Bauble" => "bauble",
                "Orb of Scouring" => "scouring",
                "Orb of Regret" => "regret",
                "Orb of Augmentation" => "augment",
                "Simple Sextant" => "sextant",
                "Prime Sextant" => "prime sextant",
                "Awakened Sextant" => "awakened sextant",
                "Orb of Annulment" => "annul",
                "Regal Orb" => "regal",
                "Armourer's Scrap" => "scrap",
                "Portal Scroll" => "portal",
                "Scroll of Wisdom" => "scroll",
                "Harbinger's Orb" => "harbinger",
                "Orb of Horizon" => "horizon",
                _ => text
            };
        }

        public static string AiCurrencyToNormzlizedCurrency(string text)
        {
            Log.Trace($"Converting AI currency to normalized currency name {text}");
            return text switch
            {
                "chaos_orb" => "chaos",
                "orb_of_alteration" => "alt",
                "orb_of_alchemy" => "alch",
                "gemcutter_prism" => "gcp",
                "exalted_orb" => "exalted",
                "chromatic_orb" => "chrome",
                "jeweller_orb" => "jewellers",
                "orb_of_chance" => "chance",
                "cartographer_chisel" => "chisel",
                "vaal_orb" => "vaal",
                "blessed_orb" => "blessed",
                "perandus_coin" => "p",
                "mirror_of_kalandra" => "mirror",
                "orb_of_transmutation" => "transmute",
                "silver_coin" => "silver",
                "orb_of_fusing" => "fuse",
                "ancient_orb" => "ancient",
                "glassblower_bauble" => "bauble",
                "orb_of_scouring" => "scouring",
                "orb_of_regret" => "regret",
                "orb_of_augmentation" => "augment",
                "simple_sextant" => "sextant",
                "prime_sextant" => "prime sextant",
                "awakened_sextant" => "awakened sextant",
                "orb_of_annulment" => "annul",
                "regal_orb" => "regal",
                "armourer_scrap" => "scrap",
                "portal_scroll" => "portal",
                "scroll_of_wisdom" => "scroll",
                "harbinger_orb" => "harbinger",
                "orb_of_horizon" => "horizon",
                _ => text
            };
        }

        public void Start()
        {
            Log.Trace("Starting CurrencyService");
        }

        #endregion
    }
}