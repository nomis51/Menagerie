using Menagerie.Shared.Models.Setting;

namespace Menagerie.Core.Services.Abstractions;

public interface ISettingsService : IService
{
    Settings GetSettings();
    void SetSettings(Settings settings);
}