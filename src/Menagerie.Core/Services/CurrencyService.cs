﻿using log4net;
using Menagerie.Core.Abstractions;
using System.Collections.Generic;
using Menagerie.Core.Extensions;

namespace Menagerie.Core.Services {
    public class CurrencyService : IService {
        #region Constants
        private static readonly ILog log = LogManager.GetLogger(typeof(CurrencyService));
        private readonly Dictionary<string, string> CurrencyToImageLink = new Dictionary<string, string>() {
            {"alt", "https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyRerollMagic.png?v=6d9520174f6643e502da336e76b730d3"},
            {"fuse","https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyRerollSocketLinks.png?v=0ad7134a62e5c45e4f8bc8a44b95540f"},
            {"alch", "https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyUpgradeToRare.png?v=89c110be97333995522c7b2c29cae728"},
            {"chaos","https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyRerollRare.png?v=c60aa876dd6bab31174df91b1da1b4f9"},
            {"gcp","https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyGemQuality.png?v=f11792b6dbd2f5f869351151bc3a4539"},
            {"exalted","https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyAddModToRare.png?v=1745ebafbd533b6f91bccf588ab5efc5"},
            {"chrome", "https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyRerollSocketColours.png?v=9d377f2cf04a16a39aac7b14abc9d7c3"},
            {"jewellers","https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyRerollSocketNumbers.png?v=2946b0825af70f796b8f15051d75164d"},
            {"chance","https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyUpgradeRandomly.png?v=e4049939b9cd61291562f94364ee0f00"},
            {"chisel", "https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyMapQuality.png?v=f46e0a1af7223e2d4cae52bc3f9f7a1f"},
            {"vaal","https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyVaal.png?v=64114709d67069cd665f8f1a918cd12a"},
            {"blessed","https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyImplicitMod.png?v=472eeef04846d8a25d65b3d4f9ceecc8"},
            {"p", "https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyCoin.png?v=b971d7d9ea1ad32f16cce8ee99c897cf"},
            {"mirror", "https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyDuplicate.png?v=6fd68c1a5c4292c05b97770e83aa22bc"},
            {"transmute","https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyUpgradeToMagic.png?v=333b8b5e28b73c62972fc66e7634c5c8"},
            {" silver","https://web.poecdn.com/image/Art/2DItems/Currency/SilverObol.png?v=93c1b204ec2736a2fe5aabbb99510bcf"}
        };
        #endregion

        #region Constructors
        public CurrencyService() {
            log.Trace("Initializing CurrencyService");
        }
        #endregion

        #region Public Methods
        public string GetCurrencyImageLink(string currencyName) {
            log.Trace($"Getting currency image link {currencyName}");
            return CurrencyToImageLink[NormalizeCurrency(currencyName)];
        }

        public string GetRealName(string text) {
            log.Trace($"Getting real currency name {text}");
            switch (text) {
                case "chaos":
                    return "Chaos Orb";

                case "alt":
                    return "Orb of Alteration";

                case "alc":
                    return "Orb of Alchemy";

                case "gcp":
                    return "Gemcutter's Prism";

                case "exalted":
                    return "Exalted Orb";

                case "chrome":
                    return "Chromatic Orb";

                case "jewellers":
                    return "Jeweller's Orb";

                case "chance":
                    return "Orb of Chance";

                case "chisel":
                    return "Cartographer's Chisel";

                case "vaal":
                    return "Vaal Orb";

                case "blessed":
                    return "Blessed Orb";

                case "p":
                    return "Perandus Coin";

                case "mirror":
                    return "Mirror of Kalandra";

                case "transmute":
                    return "Orb of Transmutation";

                case "silver":
                    return "Silver Coin";

                default:
                    return text;
            }
        }
        public string NormalizeCurrency(string text) {
            log.Trace($"Normalizing currency name {text}");
            switch (text) {
                case "Chaos Orb":
                    return "chaos";

                case "Orb of Alteration":
                    return "alt";

                case "Orb of Alchemy":
                    return "alc";

                case "Gemcutter's Prism":
                    return "gcp";

                case "Exalted Orb":
                    return "exalted";

                case "Chromatic Orb":
                    return "chrome";

                case "Jeweller's Orb":
                    return "jewellers";

                case "Orb of Chance":
                    return "chance";

                case "Cartographer's Chisel":
                    return "chisel";

                case "Vaal Orb":
                    return "vaal";

                case "Blessed Orb":
                    return "blessed";

                case "Perandus Coin":
                    return "p";

                case "Mirror of Kalandra":
                    return "mirror";

                case "Orb of Transmutation":
                    return "transmute";

                case "Silver Coin":
                    return "silver";

                default:
                    return text;
            }
        }

        public void Start() {
            log.Trace("Starting CurrencyService");
        }
        #endregion
    }
}