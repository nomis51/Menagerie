using Avalonia;
using Avalonia.ReactiveUI;
using System;
using Menagerie.Core.Services;
using Menagerie.Helpers;
using Serilog;

namespace Menagerie;

sealed class Program
{
    #region Public methods

    [STAThread]
    public static void Main(string[] args)
    {
        UpdateHelper.HookSquirrel();
        InitializeServices();

        try
        {
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            Log.Error("Unhandled exception: {Message} {StackTrace}", e.Message, e.StackTrace);
        }
    }

    #endregion

    #region Private methods

    private static void InitializeServices()
    {
        AppService.Instance.Initialize();
    }

    private static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
    }

    #endregion
}