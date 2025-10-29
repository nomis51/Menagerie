using Avalonia;
using System;
using Serilog;

namespace Menagerie;

sealed class Program
{
    #region Public methods

    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            Log.Fatal(e, "An unexpected error occurred");
        }
    }

    #endregion

    #region Private methods

    private static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    }

    #endregion
}