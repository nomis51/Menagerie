using System.Collections;

namespace Menagerie.Tests.Data;

public class ClientLogInvalidIncomingTradeLines : IEnumerable<TheoryDataRow<string>>
{
    private static readonly IEnumerable<string> Values =
    [
        string.Empty,
        "random stuff",
        "2025/10/14 09:19:02 1234567 abc123a1 [INFO Client 123] [ENGINE] Init\n",
        "202/09/14 19:12:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2/09/14 19:12:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "0/09/14 19:12:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "/09/14 19:12:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/0/14 19:12:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/1/14 19:12:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025//14 19:12:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/0 19:12:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/45 19:12:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/ 19:12:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/12 1:12:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/12 0:12:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/12 :12:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/12 45:12:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/12 09:1:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/12 09:0:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/12 09:61:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/12 09::12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/12 09:09:1 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/12 09:09:0 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/12 09:09:61 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/12 09:09:12 1234567 abc123a1 INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/12 09:09:12 1234567 abc123a1 [Client 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/12 09:09:12 1234567 abc123a1 [INFO 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/12 09:09:12 1234567 abc123a1 [INFO Nope 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/12 09:09:12 1234567 abc123a1 [INFO Client 123 @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/12 09:09:12 1234567 abc123a1 [INFO Client 123] From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/12 09:09:12 1234567 abc123a1 [INFO Client 123] @To BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/12 09:09:12 1234567 abc123a1 [INFO Client 123] BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/12 09:09:12 1234567 abc123a1 [INFO Client 123] @From : Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/12 09:09:12 1234567 abc123a1 [INFO Client 123] @From: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/12 09:09:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        // Whisper template checks
        "2025/09/12 09:09:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hej, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/12 09:09:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, jag would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/12 09:09:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I vilja ha like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/12 09:09:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like att buy your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/12 09:09:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to köpa your Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/12 09:09:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy din Betrayal's Sting, Steel Ring listed for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/12 09:09:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listad for 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/12 09:09:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed för 1 divine in Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/12 09:09:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine på Standard (stash tab \"~1 divine\"; position: left 1, top 2)\n",
        "2025/09/12 09:09:12 1234567 abc123a1 [INFO Client 123] @From BugCatcherJohn: Hi, I would like to buy your Betrayal's Sting, Steel Ring listed for 1 divine in (stash tab \"~1 divine\"; position: left 1, top 2)\n",
    ];

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