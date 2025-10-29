namespace Menagerie.Shared.Models.Parsing;

public class Token
{
    public string Value { get; }
    public string PropertyName { get; }
    public string AlternateValue { get; }
    public bool IsUseful { get; }
    public bool BreakOnFail { get; }
    public Type? TargetType { get; }
    public bool EndOfString { get; }

    public Token(string value, bool isUseful, string propertyName = "", Type? targetType = null, string alternateValue = "", bool breakOnFail = true, bool endOfString = false)
    {
        Value = value;
        PropertyName = propertyName;
        AlternateValue = alternateValue;
        IsUseful = isUseful;
        BreakOnFail = breakOnFail;
        EndOfString = endOfString;

        if (IsUseful && targetType is null) throw new Exception("Invalid parsing token");
        TargetType = targetType;
    }
}