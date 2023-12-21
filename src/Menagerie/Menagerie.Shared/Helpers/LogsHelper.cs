using Serilog;
using Serilog.Events;

namespace Menagerie.Shared.Helpers;

public static class LogsHelper
{
    #region Constants

    private const string LogsFolder = "%USERPROFILE%/Documents/My Games/Menagerie/logs/";

    #endregion

    #region Props

    public static string Location => Environment.ExpandEnvironmentVariables(LogsFolder);

    #endregion

    #region Public methods

    public static void Initialize()
    {
        if (!Directory.Exists(Location)) Directory.CreateDirectory(Location);

        var filePath = Path.Join(Location, ".txt");

        var config = new LoggerConfiguration()
            .WriteTo.File(filePath, LogEventLevel.Information, rollingInterval: RollingInterval.Day);

        var env = Environment.GetEnvironmentVariable("ENV");
        if (env is "dev" or "PoE")
        {
            config = config.WriteTo.Debug();
            config = config.WriteTo.Console();
        }

        Log.Logger = config.CreateLogger();
    }

    #endregion

}