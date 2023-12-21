namespace Menagerie.Shared.Helpers;

public static class CurrencyHelper
{
    #region Constants

    private static readonly Dictionary<string, string> CurrencyToImageLink = new()
    {
        {
            "chaos",
            "https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyRerollRare.png?v=c60aa876dd6bab31174df91b1da1b4f9"
        },
        {
            "divine",
            "https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvQ3VycmVuY3kvQ3VycmVuY3lNb2RWYWx1ZXMiLCJzY2FsZSI6MX1d/ec48896769/CurrencyModValues.png"
        },
        {
            "exalted",
            "https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyAddModToRare.png?v=1745ebafbd533b6f91bccf588ab5efc5"
        },
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
            "gcp",
            "https://web.poecdn.com/image/Art/2DItems/Currency/CurrencyGemQuality.png?v=f11792b6dbd2f5f869351151bc3a4539"
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
            "whetstone",
            "https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvQ3VycmVuY3kvQ3VycmVuY3lXZWFwb25RdWFsaXR5Iiwic2NhbGUiOjF9XQ/c9cd72719e/CurrencyWeaponQuality.png"
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
        },
        { "ex shard", "https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvQ3VycmVuY3kvRXhhbHRlZFNoYXJkIiwic2NhbGUiOjF9XQ/b9e4013af5/ExaltedShard.png" },
        {"", "https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvQ3VycmVuY3kvUnV0aGxlc3MvQ29pblBpbGVUaWVyMiIsInNjYWxlIjoxfV0/48edfd8be7/CoinPileTier2.png"}
    };

    private static readonly IEnumerable<string> TopCurrencies = new[]
    {
        "Divine Orb",
        "Chaos Orb",
    };

    #endregion

    #region Public methods

    public static List<string> GetRealCurrencyNames()
    {
        var result = CurrencyToImageLink.Keys.Select(GetRealName)
            .Where(e => !TopCurrencies.Contains(e))
            .OrderBy(e => e)
            .ToList();

        foreach (var topCurrency in TopCurrencies)
        {
            result.Insert(0, topCurrency);
        }

        return result;
    }

    public static string GetCurrencyImageLink(string currencyName)
    {
        if (CurrencyToImageLink.TryGetValue(currencyName, out var uri)) return uri;

        var norm = NormalizeCurrency(currencyName);

        return !CurrencyToImageLink.TryGetValue(norm, out var url) ? CurrencyToImageLink[""] : url;
    }

    public static string NormalizeCurrency(string text)
    {
        return text switch
        {
            "Divine Orb" => "divine",
            "Chaos Orb" => "chaos",
            "Orb of Alteration" => "alt",
            "Orb of Alchemy" => "alch",
            "Gemcutter's Prism" => "gcp",
            "Exalted Orb" => "exalted",
            "Exalted Shard" => "ex shard",
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
            "Orb of Horizon" or "Orb of Horizons" => "horizon",
            "Blacksmith's Whetstone" => "whetstone",
            _ => text
        };
    }

    public static string GetRealName(string text)
    {
        return text switch
        {
            "divine" => "Divine Orb",
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
            "whetstone" => "Blacksmith's Whetstone",
            _ => text
        };
    }

    #endregion
}