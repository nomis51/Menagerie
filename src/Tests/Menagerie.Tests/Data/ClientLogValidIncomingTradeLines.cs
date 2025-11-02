using System.Collections;

namespace Menagerie.Tests.Data;

public class ClientLogValidIncomingTradeLines : IEnumerable<TheoryDataRow<string>>
{
    private static readonly IEnumerable<string> Values =
    [
        "2025/09/12 09:09:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/12 09:09:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)",
        "2025/09/12 09:09:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard",
        "2025/09/12 09:09:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard\n",
        "2025/09/12 09:09:12 1234567 abc123a1 [INFO Client 123] @From DivinePåBordet: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/12 09:09:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy your Something That Is Not A REAL Item 02, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/12 09:09:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/12 09:09:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy your OK listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/12 09:09:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1.2 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/12 09:09:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in NOT STANDARD LEAGUE (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/12 09:09:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~å~\"; position: left 1, top 2)\n",
        "2025/09/12 09:09:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 999, top 2)\n",
        "2025/09/12 09:09:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 999)\n",
    ];

    public static readonly string[] ValidPlayerNames = ["BugCatcherJohn", "DivinePåBordet"];
    public static readonly string[] ValidCurrencies = ["divine"];
    public static readonly string[] ValidLeagues = ["Standard", "NOT STANDARD LEAGUE"];

    public static readonly string[] ValidItemNames =
        ["Betrayal's Sting, Steel Ring", "OK", "Something That Is Not A REAL Item 02, Steel Ring"];

    public static readonly float[] ValidPrices = [1, 1.2f];

    public IEnumerator<TheoryDataRow<string>> GetEnumerator()
    {
        return Values.Select(e => new TheoryDataRow<string>(e))
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}