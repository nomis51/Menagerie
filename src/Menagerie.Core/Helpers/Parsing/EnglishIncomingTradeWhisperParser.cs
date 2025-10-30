using System.Text.RegularExpressions;
using Menagerie.Core.Enums.Trading;
using Menagerie.Core.Helpers.Parsing.Abstractions;
using Menagerie.Core.Models.Trading;

namespace Menagerie.Core.Helpers.Parsing;

public class EnglishIncomingTradeWhisperParser : ITradeWhisperParser
{
    #region Constants

    private static readonly Regex RegParse =
        new(
            """/(?<time>[0-9]{4}\/[0-9]{2}\/[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2}) .* \[[a-z]+ Client [0-9]+\] @From (?<player_name>.+?): Hi, I would like to buy your (?<item_name>.+?) listed for (?:(?<price>\d+(?:\.\d+)?)\s*)?(?<currency>.+?) in (?<league>.+?) \(stash tab "(?<stash_tab_name>.+?)"; position: left (?<stash_tab_left>[0-9]+), top (?<stash_tab_top>[0-9]+)\)/""",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.NonBacktracking
        );

    #endregion

    #region Public methods

    public Trade? Parse(string line)
    {
        // 2025/10/29 16:34:47 1726297 cff945b9 [INFO Client 312] @From EsquentaPraVinteSete: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab "Tabarnak"; position: left 7, top 8)
        var match = RegParse.Match(line);
        if (!match.Success) return null;
        if (!match.Groups.ContainsKey("time") ||
            !match.Groups["time"].Success ||
            !DateTime.TryParse(match.Groups["time"].Value, out var time)) return null;
        if (!match.Groups.ContainsKey("player_name") ||
            !match.Groups["player_name"].Success) return null;
        if (!match.Groups.ContainsKey("item_name") ||
            !match.Groups["item_name"].Success) return null;
        if (!match.Groups.ContainsKey("price") ||
            !match.Groups["price"].Success ||
            !float.TryParse(match.Groups["price"].Value, out var price)) return null;
        if (!match.Groups.ContainsKey("currency") ||
            !match.Groups["currency"].Success) return null;
        if (!match.Groups.ContainsKey("league") ||
            !match.Groups["league"].Success) return null;

        return new Trade
        {
            Id = 0,
            Type = TradeType.Incoming,
            Whisper = line,
            ItemName = match.Groups["item_name"].Value,
            League = match.Groups["league"].Value,
            PlayerName = match.Groups["player_name"].Value,
            Time = time,
            Price = price,
            Currency = match.Groups["currency"].Value,
            StashTab = new StashTabLocation
            {
                Name = !match.Groups.ContainsKey("stash_tab_name") ||
                       !match.Groups["stash_tab_name"].Success
                    ? null
                    : match.Groups["stash_tab_name"].Value,
                Left = !match.Groups.ContainsKey("stash_tab_left") ||
                       !match.Groups["stash_tab_left"].Success ||
                       !int.TryParse(match.Groups["stash_tab_left"].Value, out var left)
                    ? 0
                    : left,
                Top = !match.Groups.ContainsKey("stash_tab_top") ||
                      !match.Groups["stash_tab_top"].Success ||
                      !int.TryParse(match.Groups["stash_tab_top"].Value, out var top)
                    ? 0
                    : top,
            }
        };
    }

    #endregion
}