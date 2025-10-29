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
        AddHelpers(services);
        AddViews(services);
        return services;
    }

    #endregion

    #region Private methods

    private static void AddLogging(IServiceCollection services)
    {
        var config = new LoggerConfiguration()
            .WriteTo.File("./logs/.txt", rollingInterval: RollingInterval.Day);

#if DEBUG
        config = config.WriteTo.Debug();
#endif

        Log.Logger = config.CreateLogger();
    }

    private static void AddViews(IServiceCollection services)
    {
        services.AddTransient<MainWindowViewModel>();
    }

    private static void AddHelpers(IServiceCollection services)
    {
    }

    #endregion
}