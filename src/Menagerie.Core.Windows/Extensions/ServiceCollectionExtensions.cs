using Menagerie.Core.Windows.Helpers;
using Menagerie.Core.Windows.Helpers.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Menagerie.Core.Windows.Extensions;

public static class ServiceCollectionExtensions
{
    #region Public methods

    public static IServiceCollection AddWindowsPlatformCapabilities(this IServiceCollection services)
    {
        services.AddSingleton<IClipboardHelper, ClipboardHelper>();
        return services;
    }

    #endregion
}