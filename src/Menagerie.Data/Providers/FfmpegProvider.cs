using System.Diagnostics;
using MediaToolkit;
using Menagerie.Data.Events;
using Menagerie.Data.Services;
using Menagerie.Shared.Helpers;
using Menagerie.Shared.Models.Setting;
using Serilog;

namespace Menagerie.Data.Providers;

public class FfmpegProvider : IDisposable
{
    #region Constants

    private const string FfmpegPath = "./tools/ffmpeg/menagerie-ffmpeg.exe";
    private const string FfmpegLogFilePath = "%USERPROFILE%/Documents/My Games/Menagerie/logs/ffmpeg.txt";
    private const string FramesFolder = "./assets/frames";
    private const string RecordFileName = "%d.mp4";

    #endregion

    #region Members

    private readonly string _ffmpegLogFilePath;
    private readonly RecordingSettings _options;
    private readonly string _windowTitle;
    private Process? _recordingProcess;
    private Process? _trimmingProcess;
    private Process? _mergingProcess;
    private Thread? _cleanupThread;
    private bool _isRecording;
    private bool _isExiting;
    private readonly object _recordingProcessLock = new();

    #endregion

    #region Constructors

    public FfmpegProvider(RecordingSettings options, string windowTitle)
    {
        if (Directory.Exists(FramesFolder)) Directory.Delete(FramesFolder, true);
        Directory.CreateDirectory(FramesFolder);

        _ffmpegLogFilePath = Environment.ExpandEnvironmentVariables(FfmpegLogFilePath);
        _windowTitle = windowTitle;
        _options = options;

        DataEvents.OnApplicationExit += OnAppExit;
    }

    #endregion

    #region Public methods

    public void Save(string clipName, int duration = 10, string outputPath = "")
    {
        Log.Information("Saving clip {Name} {Duration}s {Path}", clipName, duration, outputPath);

        Thread.Sleep(_options.ClipSaveDelay * 1000);

        lock (_recordingProcessLock)
        {
            Stop(false);

            var defaultOutputPath = Environment.ExpandEnvironmentVariables(string.IsNullOrEmpty(outputPath) ? _options.OutputPath : outputPath);
            if (!Directory.Exists(defaultOutputPath))
            {
                Directory.CreateDirectory(defaultOutputPath);
            }

            var clipFilePath = Path.Join(defaultOutputPath, $"{clipName}.mp4");
            var paddedDuration = duration + _options.ClipSavePadding;

            var filesIds = Directory.EnumerateFiles(FramesFolder)
                .Select(f => int.Parse(Path.GetFileName(f).Split('.')[0]))
                .OrderByDescending(e => e)
                .ToList();
            if (!filesIds.Any()) return;

            var latestClipFilePath = $"{FramesFolder}/{RecordFileName.Replace("%d", filesIds.First().ToString())}";
            var latestClipDuration = GetFileDuration(latestClipFilePath);
            var latestClipDiff = (int)Math.Round(latestClipDuration.TotalSeconds - paddedDuration);

            if (latestClipDiff < 0 && filesIds.Count > 1)
            {
                HandleShortClip(filesIds[1], filesIds[0], paddedDuration, clipFilePath);
            }
            else if (latestClipDiff > 1.0)
            {
                HandleLongClip(latestClipDiff, paddedDuration, latestClipFilePath, clipFilePath);
            }
            else if (latestClipDiff is 0 or <= 1 || filesIds.Count == 1)
            {
                HandleClip(latestClipFilePath, clipFilePath);
            }
        }
    }

    public void Stop(bool removeClips = true)
    {
        if (_recordingProcess is not null && !_recordingProcess.HasExited)
        {
            _recordingProcess.StandardInput.Write('q');
            _recordingProcess.WaitForExit();
        }

        _isRecording = false;

        if (!removeClips) return;

        RemoveClips();
    }

    public void Record()
    {
        RemoveClips();
        _isRecording = true;
        StartCleanupThread();
        SpawnRecordProcess();
    }

    public void Dispose()
    {
        _recordingProcess?.Dispose();
        _trimmingProcess?.Dispose();
        _mergingProcess?.Dispose();
        RemoveClips();
    }

    #endregion

    #region Private methods

    private void StartCleanupThread()
    {
        _cleanupThread = new Thread(AutoRemoveOldClips)
        {
            IsBackground = true
        };
        _cleanupThread.Start();
    }

    private void HandleClip(string clipFilePath, string outputFilePath)
    {
        File.Copy(clipFilePath, outputFilePath);
    }

    private void HandleLongClip(int start, int duration, string clipFilePath, string outputFilePath)
    {
        Log.Information("Long clip, trimming {Path} {Duration}s", clipFilePath, duration);
        SpawnTrimProcess(start, duration, clipFilePath, outputFilePath).Wait();
    }

    private void HandleShortClip(int clip1Id, int clip2Id, int duration, string outputFilePath)
    {
        Log.Information("Short clip, merging {A} and {B} for {Duration}s", clip1Id, clip2Id, duration);
        var mergedFilePath = $"{FramesFolder}/{RecordFileName.Replace("%d", "merge")}";
        var fileName1 = $"{RecordFileName.Replace("%d", clip1Id.ToString())}";
        var fileName2 = $"{RecordFileName.Replace("%d", clip2Id.ToString())}";
        SpawnMergeProcess(fileName1, fileName2, mergedFilePath).Wait();

        var clipDuration1 = GetFileDuration($"{FramesFolder}/{fileName1}");
        var clipDuration2 = GetFileDuration($"{FramesFolder}/{fileName2}");
        var trimStartTime = clipDuration2.TotalSeconds + clipDuration1.TotalSeconds - duration;
        SpawnTrimProcess(trimStartTime <= 0 ? 0 : (int)trimStartTime, duration, mergedFilePath, outputFilePath).Wait();
        File.Delete(mergedFilePath);
    }

    private void AutoRemoveOldClips()
    {
        while (_isRecording)
        {
            Thread.Sleep(_options.CleanupTimeout * 1000);
            RemoveOldClips();
        }
    }

    private void RemoveOldClips()
    {
        var filesIds = Directory.EnumerateFiles(FramesFolder)
            .Select(f =>
            {
                if (int.TryParse(Path.GetFileName(f).Split('.')[0], out var intValue)) return intValue;
                return -1;
            })
            .Where(v => v != -1)
            .OrderBy(e => e)
            .ToList();
        if (!filesIds.Any()) return;

        Log.Information("Removing old {Count} clips", filesIds.Count);

        for (var i = 0; i < filesIds.Count; ++i)
        {
            if (i <= _options.NbClipsToKeep) break;

            try
            {
                File.Delete($"{FramesFolder}/{RecordFileName.Replace("%d", filesIds[i].ToString())}");
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }

    private void RemoveClips()
    {
        foreach (var file in Directory.EnumerateDirectories(FramesFolder))
        {
            File.Delete(file);
        }
    }

    private TimeSpan GetFileDuration(string filePath)
    {
        var inputFile = new MediaToolkit.Model.MediaFile { Filename = filePath };
        using var engine = new Engine();
        engine.GetMetadata(inputFile);
        return inputFile.Metadata.Duration;
    }

    private Task SpawnMergeProcess(string filePath1, string filePath2, string outputFilePath)
    {
        const string filesFilePath = $"{FramesFolder}/files.txt";
        File.WriteAllText(filesFilePath, $"file {filePath1}\r\nfile {filePath2}");

        _mergingProcess = ProcessHelper.SpawnProcess(new ProcessStartInfo
        {
            WindowStyle = ProcessWindowStyle.Hidden,
            FileName = FfmpegPath,
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true,
            Arguments =
                $"-f concat -safe 0 -i \"{filesFilePath}\" -y -c copy \"{outputFilePath}\""
        }, _ffmpegLogFilePath);

        Task.Run(() =>
        {
            _mergingProcess.WaitForExit();
            File.Delete(filesFilePath);
        });

        return _mergingProcess.WaitForExitAsync();
    }

    private Task SpawnTrimProcess(int start, int duration, string filePath, string outputFilePath)
    {
        _trimmingProcess = ProcessHelper.SpawnProcess(new ProcessStartInfo
        {
            WindowStyle = ProcessWindowStyle.Hidden,
            FileName = FfmpegPath,
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true,
            Arguments =
                $"-ss {start} -i \"{filePath}\" -t {duration} -y -c copy \"{outputFilePath}\""
        }, _ffmpegLogFilePath);

        return _trimmingProcess.WaitForExitAsync();
    }

    private void SpawnRecordProcess()
    {
        new Thread(() =>
        {
            while (!_isExiting)
            {
                lock (_recordingProcessLock)
                {
                    if (_recordingProcess is not null && !_recordingProcess!.HasExited) _recordingProcess.Kill();

                    var cores = FindNbCores();

                    _recordingProcess = ProcessHelper.SpawnProcess(new ProcessStartInfo
                    {
                        WindowStyle = ProcessWindowStyle.Hidden,
                        FileName = FfmpegPath,
                        UseShellExecute = false,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                        Arguments =
                            $"-f gdigrab -i title=\"{_windowTitle}\" -r {_options.FrameRate} -y -draw_mouse 0 -threads {cores} -rtbufsize 100M -probesize 10M -preset ultrafast -tune zerolatency -c:v libx264 -crf {_options.Crf} -pix_fmt yuv420p -f segment -segment_time {_options.ClipDuration} -force_key_frames \"expr:not(mod(n,{_options.ClipDuration}))\" -t 02:00:00 -reset_timestamps 1 {FramesFolder}/{RecordFileName}"
                    }, _ffmpegLogFilePath);
                }

                _recordingProcess.WaitForExit();
            }
        })
        {
            IsBackground = true
        }.Start();
    }

    private int FindNbCores()
    {
        var settings = AppDataService.Instance.GetSettings();
        if (int.TryParse(settings.Recording.Threads, out var intValue) && intValue <= Environment.ProcessorCount && intValue > 0) return intValue;

        var cores = Math.Floor(Environment.ProcessorCount / 4.0d);
        if (cores == 0) return 1;
        return (int)cores;
    }

    private void OnAppExit()
    {
        _isExiting = true;
        Dispose();
    }

    #endregion
}