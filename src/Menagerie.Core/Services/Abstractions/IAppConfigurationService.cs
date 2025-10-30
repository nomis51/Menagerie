using Menagerie.Core.Models.Configuration;

namespace Menagerie.Core.Services.Abstractions;

public interface IAppConfigurationService
{
    Task<AppConfiguration> GetConfigurationAsync();
    Task SaveConfigurationAsync(AppConfiguration configuration);
}