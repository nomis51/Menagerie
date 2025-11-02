using Menagerie.Core.Enums.Trading;
using Menagerie.Core.Helpers.Parsing;
using Menagerie.Tests.Data;
using Shouldly;

namespace Menagerie.Tests.Tests.Helpers.Parsing;

public class EnglishIncomingTradeWhisperParserTests
{
    #region Members

    private readonly EnglishIncomingTradeWhisperParser _sut = new();
    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public EnglishIncomingTradeWhisperParserTests(ITestOutputHelper output)
    {
        _output = output;
    }

    #endregion

    #region Tests

    [Theory]
    [ClassData(typeof(ClientLogInvalidIncomingTradeLines))]
    public void Parse_ShouldReturnNull(string input)
    {
        // Arrange
        _output.WriteLine($"Testing input: {input}");

        // Act
        var result = _sut.Parse(input);

        // Assert
        result.ShouldBeNull();
    }

    [Theory]
    [ClassData(typeof(ClientLogValidIncomingTradeLines))]
    public void Parse_ShouldReturnTrade(string input)
    {
        // Arrange
        _output.WriteLine($"Testing input: {input}");

        // Act
        var result = _sut.Parse(input);

        // Assert
        result.ShouldNotBeNull();
        ClientLogValidIncomingTradeLines.ValidCurrencies.ShouldContain(result.Currency);
        ClientLogValidIncomingTradeLines.ValidItemNames.ShouldContain(result.ItemName);
        ClientLogValidIncomingTradeLines.ValidLeagues.ShouldContain(result.League);
        ClientLogValidIncomingTradeLines.ValidPlayerNames.ShouldContain(result.PlayerName);
        ClientLogValidIncomingTradeLines.ValidPrices.ShouldContain(result.Price);
        result.StashTab.Name.ShouldBe("~1 divine");
        result.StashTab.Left.ShouldBe(1);
        result.StashTab.Top.ShouldBe(2);
        result.State.ShouldBe(TradeState.Initial);
        result.Time.ShouldNotBe(DateTime.MinValue);
        result.Whisper.ShouldBe(input.Split("@From", StringSplitOptions.TrimEntries).Last());
        result.Type.ShouldBe(TradeType.Incoming);
        result.Id.ShouldBeGreaterThan(0);
    }

    #endregion
}