using Menagerie.Core.Linux.Helpers;
using Menagerie.Core.Linux.Helpers.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Menagerie.Core.Linux.Extensions;

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