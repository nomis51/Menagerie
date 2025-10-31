using Menagerie.Core.Helpers.Parsing;
using Menagerie.Tests.Data;
using Shouldly;

namespace Menagerie.Tests.Tests.Helpers.Parsing;

public class EnglishIncomingTradeWhisperParserTests
{
    #region Members

    private readonly EnglishIncomingTradeWhisperParser _sut = new();

    #endregion

    #region Tests

    [Theory]
    [ClassData(typeof(ClientLogInvalidIncomingTradeLines))]
    public void Parse_ShouldReturnNull(string input)
    {
        // Arrange

        // Act
        var result = _sut.Parse(input);

        // Assert
        result.ShouldBeNull();
    }

    #endregion
}