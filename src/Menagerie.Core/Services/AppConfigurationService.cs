using System.Runtime.InteropServices;
using System.Text.Json;
using Menagerie.Core.Models.Configuration;
using Menagerie.Core.Services.Abstractions;
using Microsoft.Extensions.Logging;

namespace Menagerie.Core.Services;

public class AppConfigurationService : IAppConfigurationService
{
    #region Constants

    private const string FileName = "config.json";

    private static readonly string Folder = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
        ? Path.Join(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            $".{nameof(Menagerie).ToLower()}"
        )
        : Path.Join(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".config",
            nameof(Menagerie).ToLower()
        );

    #endregion

    #region Members

    private readonly SemaphoreSlim _fileLock = new(1, 1);
    private readonly ILogger<AppConfigurationService> _logger;

    #endregion

    #region Props

    private static string FilePath => Path.Combine(Folder, FileName);

    #endregion

    #region Constructors

    public AppConfigurationService(ILogger<AppConfigurationService> logger)
    {
        _logger = logger;
    }

    #endregion

    #region Public methods

    public async Task<AppConfiguration> GetConfigurationAsync()
    {
        await _fileLock.WaitAsync();

        try
        {
            EnsureFolderExists();

            if (!File.Exists(FilePath)) return new AppConfiguration();

            var json = await File.ReadAllTextAsync(FilePath);
            if (string.IsNullOrWhiteSpace(json)) return new AppConfiguration();

            return JsonSerializer.Deserialize<AppConfiguration>(json) ?? new AppConfiguration();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while reading configuration file");
            return new AppConfiguration();
        }
        finally
        {
            _fileLock.Release();
        }
    }

    public async Task SaveConfigurationAsync(AppConfiguration configuration)
    {
        await _fileLock.WaitAsync();

        try
        {
            EnsureFolderExists();

            var json = JsonSerializer.Serialize(configuration);
            await File.WriteAllTextAsync(FilePath, json);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while writing configuration file");
        }
        finally
        {
            _fileLock.Release();
        }
    }

    #endregion

    #region Private methods

    private static void EnsureFolderExists()
    {
        if (!Directory.Exists(Folder))
        {
            Directory.CreateDirectory(Folder);
        }
    }

    #endregion
}