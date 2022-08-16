using System.Text;
using Menagerie.Shared.Abstractions;
using Menagerie.Shared.Models.Setting;
using Newtonsoft.Json;

namespace Menagerie.Data.Services;

public class SettingsService : IService
{
    #region Constants

    private const string SettingsFile = "settings.json";
    private const string SettingsFolder = "%USERPROFILE%/Documents/My Games/Menagerie/";

    #endregion

    #region Members

    private readonly string _settingsFilePath;
    private Settings _settings;

    #endregion

    #region Constructors

    public SettingsService()
    {
        var path = Environment.ExpandEnvironmentVariables(SettingsFolder);
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        
        _settingsFilePath = Path.Join(path, SettingsFile);
    }

    #endregion

    #region Public methods

    public void Initialize()
    {
        if (!File.Exists(_settingsFilePath))
        {
            CreateDefaultSettings();
        }
        else
        {
            var settingsData = File.ReadAllText(_settingsFilePath);
            if (string.IsNullOrEmpty(settingsData))
            {
                CreateDefaultSettings();
            }
            else
            {
                try
                {
#pragma warning disable CS8601
                    _settings = JsonConvert.DeserializeObject<Settings>(settingsData);
#pragma warning restore CS8601
                }
                catch (Exception)
                {
                    CreateDefaultSettings();
                }
            }
        }

    }

    public Task Start()
    {
        return Task.CompletedTask;
    }

    public Settings GetSettings()
    {
        return _settings;
    }

    public void SetSettings(Settings settings)
    {
        _settings = settings;
        _ = Task.Run(WriteSettings);
    }

    #endregion

    #region Private methods

    private void WriteSettings()
    {
        var settingsData = JsonConvert.SerializeObject(_settings);
        File.WriteAllText(_settingsFilePath, settingsData, Encoding.UTF8);
    }

    private void CreateDefaultSettings()
    {
        _settings = new Settings();
        WriteSettings();
    }

    #endregion
}