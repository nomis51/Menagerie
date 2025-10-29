using System.Reflection;
using System.Text;
using Menagerie.Data.WinApi;
using Menagerie.Shared.Abstractions;
using Menagerie.Shared.Helpers;
using Menagerie.Shared.Models.Setting;
using Newtonsoft.Json;
using Serilog;

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
        Log.Warning("Application starting {Version} {Id}", GetVersion(), _settings.Id);
        return Task.CompletedTask;
    }

    public Settings GetSettings()
    {
        return _settings;
    }

    public void SetSettings(Settings settings)
    {
        _settings = settings;
        _ = Task.Run(() => WriteSettings());
    }

    #endregion

    #region Private methods

    private void WriteSettings(int count = 0)
    {
        try
        {
            var settingsData = JsonConvert.SerializeObject(_settings);
            File.WriteAllText(_settingsFilePath, settingsData, Encoding.UTF8);
        }
        catch (Exception e)
        {
            Log.Warning("Unable to write settings {Message}: ", e.Message);

            if (count == 1)
            {
                User32.MessageBox(IntPtr.Zero, $"Unable to save settings: {e.Message}", "Menagerie", 0x00000030 | 0x00000000); // warning icon + ok button
                return;
            }

            Thread.Sleep(3000);
            ProcessHelper.CleanUnexpectedProcesses().Wait();
            WriteSettings(1);
        }
    }

    private string GetVersion()
    {
        try
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            return version is null
                ? string.Empty
                : $"Application Version: {version.Major}.{version.Minor}.{version.Build}";
        }
        catch (Exception)
        {
            Log.Warning("Unable to retrieve app version");
            return string.Empty;
        }
    }

    private void CreateDefaultSettings()
    {
        _settings = new Settings();
        WriteSettings();
    }

    #endregion
}