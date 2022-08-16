using System.Text.RegularExpressions;
using Menagerie.Data.Providers;
using Menagerie.Shared.Abstractions;
using Serilog;

namespace Menagerie.Data.Services;

public class RecordingService : IService
{
    #region Members

    private FfmpegProvider? _ffmpegProvider;

    #endregion

    #region Public methods

    public void Initialize()
    {
        var settings = AppDataService.Instance.GetSettings();

        if (!settings.Recording.Enabled) return;
        _ffmpegProvider = new FfmpegProvider(settings.Recording, "Path of Exile");
    }

    public Task Start()
    {
        var settings = AppDataService.Instance.GetSettings();

        if (settings.Recording.Enabled) Record();
        return Task.CompletedTask;
    }

    public void SaveDeathClip(string characterName, string zoneName)
    {
        var settings = AppDataService.Instance.GetSettings();
        if (!settings.Recording.Enabled) return;

        Save(
            RemoveInvalidCharacters($"death-{characterName}-{(string.IsNullOrEmpty(zoneName) ? string.Empty : $"{zoneName}-")}{DateTime.Now.ToString("yyyyMMdd-hhmmss")}"),
            settings.DeathReplay.Duration,
            settings.DeathReplay.OutputPath
        );
    }

    public void SaveClip()
    {
        var settings = AppDataService.Instance.GetSettings();
        if (!settings.Recording.Enabled) return;

        Save(
            $"replay-{DateTime.Now.ToString("yyyyMMdd-hhmmss")}",
            settings.Replay.Duration,
            settings.Replay.OutputPath
        );
    }

    public void Record()
    {
        _ffmpegProvider.Record();
    }

    public void Stop()
    {
        _ffmpegProvider.Stop();
    }

    public void Save(string filename, int duration, string outputPath = "")
    {
        _ffmpegProvider.Save(filename, duration, outputPath);
    }

    #endregion

    #region Private methods

    private string RemoveInvalidCharacters(string input)
    {
        var regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        var r = new Regex($"[{Regex.Escape(regexSearch)}]");
        return r.Replace(input, "");
    }

    #endregion
}