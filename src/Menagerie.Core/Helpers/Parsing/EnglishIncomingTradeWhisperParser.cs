using System.Text.RegularExpressions;
using Menagerie.Core.Enums.Trading;
using Menagerie.Core.Helpers.Parsing.Abstractions;
using Menagerie.Core.Models.Trading;

namespace Menagerie.Core.Helpers.Parsing;

public class EnglishIncomingTradeWhisperParser : ITradeWhisperParser
{
    #region Constants

    private static readonly Regex RegParse = new(
        @"(?<time>[0-9]{4}\/[0-9]{2}\/[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2}) .* \[[a-zA-Z]+ Client [0-9]+\] @From (?<player_name>.+?): Hi, I would like to buy your (?<item_name>.+?) listed for (?:(?<price>\d+(?:\.\d+)?)\s+){0,1}(?<currency>.+?) in (?<league>.+?)(?: \(stash tab ""(?<stash_tab_name>.+?)""; position: left (?<stash_tab_left>[0-9]+), top (?<stash_tab_top>[0-9]+)\))?(\\n|\n)*",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline
    );

    #endregion

    #region Public methods

    public Trade? Parse(string line)
    {
        var match = RegParse.Match(line);
        if (!match.Success) return null;
        if (!match.Groups.ContainsKey("time") ||
            !match.Groups["time"].Success ||
            !DateTime.TryParse(match.Groups["time"].Value, out var time)) return null;
        if (!match.Groups.ContainsKey("player_name") ||
            !match.Groups["player_name"].Success) return null;
        if (!match.Groups.ContainsKey("item_name") ||
            !match.Groups["item_name"].Success) return null;

        var price = 0f;
        if (match.Groups["price"].Success &&
            !float.TryParse(match.Groups["price"].Value, out price)) return null;
        if (!match.Groups.ContainsKey("currency") ||
            !match.Groups["currency"].Success) return null;
        if (!match.Groups.ContainsKey("league") ||
            !match.Groups["league"].Success ||
            match.Groups["league"].Value.Contains('(')) return null;

        string? stashTab = null;
        if (match.Groups.ContainsKey("stash_tab_name") &&
            match.Groups["stash_tab_name"].Success)
        {
            stashTab = match.Groups["stash_tab_name"].Value;
        }

        var left = -1;
        if (match.Groups.ContainsKey("stash_tab_left") &&
            match.Groups["stash_tab_left"].Success &&
            int.TryParse(match.Groups["stash_tab_left"].Value, out left))
        {
        }

        var top = -1;
        if (match.Groups.ContainsKey("stash_tab_top") &&
            match.Groups["stash_tab_top"].Success &&
            int.TryParse(match.Groups["stash_tab_top"].Value, out top))
        {
        }

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
                Name = stashTab,
                Left = left,
                Top = top
            }
        };
    }

    #endregion
}