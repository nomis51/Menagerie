﻿using log4net;
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
                " silver",
                "https://web.poecdn.com/image/Art/2DItems/Currency/SilverObol.png?v=93c1b204ec2736a2fe5aabbb99510bcf"
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

            var link = _currencyToImageLink[NormalizeCurrency(currencyName)];

            var dbImage = AppService.Instance.GetImage(link);

            if (dbImage != null) return dbImage.Base64;
            dbImage = new Models.AppImage()
            {
                Link = link,
                Base64 = GetImage(link)
            };

            AppService.Instance.SaveImage(dbImage);

            return dbImage.Base64;
        }

        public static string GetRealName(string text)
        {
            Log.Trace($"Getting real currency name {text}");
            return text switch
            {
                "chaos" => "Chaos Orb",
                "alt" => "Orb of Alteration",
                "alc" => "Orb of Alchemy",
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
                _ => text
            };
        }

        private static string NormalizeCurrency(string text)
        {
            Log.Trace($"Normalizing currency name {text}");
            return text switch
            {
                "Chaos Orb" => "chaos",
                "Orb of Alteration" => "alt",
                "Orb of Alchemy" => "alc",
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