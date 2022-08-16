using Serilog;
using Serilog.Events;

namespace Menagerie.Shared.Helpers;

public class LogHelper
{
    #region Constants

    private const string LogsFolder = "%USERPROFILE%/Documents/My Games/Menagerie/logs/";

    #endregion

    #region Public methods

    public static void Initialize()
    {
        var path = Environment.ExpandEnvironmentVariables(LogsFolder);
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        var filePath = Path.Join(path, ".txt");

        var config = new LoggerConfiguration()
            .WriteTo.File(filePath, LogEventLevel.Information, rollingInterval: RollingInterval.Day);

        var env = Environment.GetEnvironmentVariable("ENV");
        if (env is "dev" or "PoE")
        {
            config = config.WriteTo.Debug();
            config = config.WriteTo.Console();
        }
        else
        {
            config = config.WriteTo.Sentry(cfg =>
            {
                cfg.MinimumBreadcrumbLevel = LogEventLevel.Debug;
                cfg.MinimumEventLevel = LogEventLevel.Warning;
                cfg.Dsn = "https://285d06119cd74b55af61566a71e5bb93@o1179575.ingest.sentry.io/6291842";
            });
        }

        Log.Logger = config.CreateLogger();
    }

    #endregion

    #region Private methods

    #endregion
}