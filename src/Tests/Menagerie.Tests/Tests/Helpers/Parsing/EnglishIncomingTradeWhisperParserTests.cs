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

        // Act
        var result = _sut.Parse(input);

        // Assert
        result.ShouldNotBeNull();
    }

    #endregion
}