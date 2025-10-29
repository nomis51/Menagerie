using System.Text.RegularExpressions;
using Menagerie.Data.Parsers.Abstractions;
using Menagerie.Shared.Models;
using Menagerie.Shared.Models.Parsing;

namespace Menagerie.Data.Parsers;

public abstract class Parser<T> : IParser
{
    #region Members

    private readonly Regex _regexCanParse;
    private readonly List<Token> _tokens;

    #endregion

    #region Constructors

    protected Parser(Regex canParse, List<Token> tokens)
    {
        _regexCanParse = canParse;
        _tokens = tokens;
    }

    #endregion

    #region Public methods

    public virtual bool CanParse(string text)
    {
        return _regexCanParse.IsMatch(text);
    }

    public T Parse(string text)
    {
        var returnValue = (T)Activator.CreateInstance(typeof(T))!;

        foreach (var token in _tokens)
        {
            var index = token.Value == string.Empty
                ? text.Length
                : text.IndexOf(token.Value, StringComparison.Ordinal);

            var usedAlternativeValue = false;

            if (index == -1 || index >= text.Length)
            {
                if (!token.EndOfString)
                {
                    if (!string.IsNullOrEmpty(token.AlternateValue))
                    {
                        usedAlternativeValue = true;
                        index = text.IndexOf(token.AlternateValue, StringComparison.Ordinal);

                        if (index == -1 || index >= text.Length)
                        {
                            if (!token.BreakOnFail) continue;
                            break;
                        }
                    }
                    else
                    {
                        if (!token.BreakOnFail) continue;
                        break;
                    }
                }
                else
                {
                    index = text.Length;
                }
            }

            var value = text[..index];
            var offset = usedAlternativeValue ? token.AlternateValue.Length : token.Value.Length;
            if (offset + index > text.Length)
            {
                offset = text.Length - index;
            }
            text = text[(index + offset)..];

            if (!token.IsUseful || token.TargetType is null) continue;

            returnValue.GetType().GetProperty(token.PropertyName)?.SetValue(returnValue, value);
        }

        return returnValue;
    }

    #endregion
}