using System.Text.RegularExpressions;
using Menagerie.Shared.Models.Chat;
using Menagerie.Shared.Models.Parsing;

namespace Menagerie.Core.Parsers;

public class LocationParser : Parser<LocationChangeEvent>
{
    public LocationParser() : base(
        new Regex(": You have entered .+\\.", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new List<Token>
        {
            new(": You have entered ", false),
            new (".", true, "Location", typeof(string))
        }
    )
    {
    }
}