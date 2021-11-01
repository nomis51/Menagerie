namespace Menagerie.Core.Models.Parsing.Enums
{
    public enum LogEntryTag
    {
        Warn,
        Info,
        Debug
    }

    public static class LogEntryTagConveter
    {
        public static LogEntryTag Convert(string str)
        {
            return str.ToLower() switch
            {
                "warn" => LogEntryTag.Warn,
                "debug" => LogEntryTag.Debug,
                _ => LogEntryTag.Info
            };
        }
    }
}