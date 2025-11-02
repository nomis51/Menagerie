using System.IO.Abstractions;
using Menagerie.Core;
using Menagerie.Core.Linux.Extensions;
using Menagerie.Core.Services;
using Menagerie.Core.Services.Abstractions;
using Menagerie.Core.Windows.Extensions;
using Menagerie.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Menagerie.Extensions;

public static class ServiceCollectionExtensions
{
    #region Public methods

    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        AddLogging(services);
        AddServices(services);
        AddHelpers(services);
        AddViews(services);
        AddPlatformCapabilities(services);
        return services;
    }

    #endregion

    #region Private methods

    private static void AddPlatformCapabilities(IServiceCollection services)
    {
        services.AddWindowsPlatformCapabilities();
        services.AddLinuxPlatformCapabilities();
    }

    private static void AddHelpers(IServiceCollection services)
    {
        services.AddSingleton<PlatformCapabilities>();
        services.AddScoped<IFileSystem, FileSystem>();
    }

    private static void AddLogging(IServiceCollection services)
    {
        services.AddLogging();

        var config = new LoggerConfiguration();

#if DEBUG
        config = config.WriteTo.Debug();
#else
        config = config.WriteTo.File("./logs/.txt", rollingInterval: RollingInterval.Day);
#endif

        Log.Logger = config.CreateLogger();
    }

    private static void AddViews(IServiceCollection services)
    {
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<IncomingTradesWindowViewModel>();
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddSingleton<IKeyboardService, KeyboardService>();
        services.AddSingleton<IAudioService, AudioService>();
        services.AddSingleton<IClientLogService, ClientLogService>();
        services.AddSingleton<IGameService, GameService>();
        services.AddSingleton<IAppConfigurationService, AppConfigurationService>();
        services.AddSingleton<IWindowService, WindowService>();
        services.AddSingleton<ITextParsingService, TextParsingService>();
        services.AddSingleton<IGameChatService, GameChatService>();
        services.AddSingleton<IClipboardService, ClipboardService>();
    }

    #endregion
}