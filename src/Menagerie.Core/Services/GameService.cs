using System.ComponentModel;
using System.Diagnostics;
using Menagerie.Core.Services.Abstractions;
using Microsoft.Extensions.Logging;

namespace Menagerie.Core.Services;

public class GameService : IGameService
{
    #region Constants

    private const string GameClientLogFileName = "Client.txt";

    #endregion

    #region Events

    public EventHandler<int>? GameProcessChanged { get; set; }

    #endregion

    #region Members

    private readonly ILogger<GameService> _logger;
    private readonly IAppConfigurationService _appConfigurationService;
    private readonly IWindowService _windowService;

    private Process? _gameProcess;
    private readonly SemaphoreSlim _gameProcessLock = new(1, 1);

    private readonly SemaphoreSlim _focusLock = new(1, 1);

    #endregion

    #region Props

    public int GameProcessId => _gameProcess?.Id ?? 0;

    private bool HasGameProcess => _gameProcess is not null &&
                                   !_gameProcess.HasExited &&
                                   _gameProcess.MainWindowHandle != IntPtr.Zero;

    #endregion

    #region Constructors

    public GameService(
        ILogger<GameService> logger,
        IAppConfigurationService appConfigurationService,
        IWindowService windowService
    )
    {
        _logger = logger;
        _appConfigurationService = appConfigurationService;
        _windowService = windowService;
    }

    #endregion

    #region Public methods

    public string? GetGameClientLogFilePathAsync()
    {
        try
        {
            if (!HasGameProcess ||
                _gameProcess!.MainModule is null ||
                string.IsNullOrEmpty(_gameProcess.MainModule.FileName)) return null;

            var filePath = Path.Join(
                Path.GetDirectoryName(_gameProcess.MainModule.FileName),
                "logs",
                GameClientLogFileName
            );
            return !File.Exists(filePath) ? null : filePath;
        }
        catch (Win32Exception)
        {
            _logger.LogError("32 bits game process not supported");
            return null;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting game client log file path");
            return null;
        }
    }

    public async Task<bool> FocusOverlayAsync()
    {
        if (!HasGameProcess) return false;

        await _focusLock.WaitAsync();

        try
        {
            // TODO:
            return false;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while focusing overlay");
            return false;
        }
        finally
        {
            _focusLock.Release();
        }
    }

    public async Task<bool> FocusGameAsync()
    {
        if (!HasGameProcess) return false;

        await _focusLock.WaitAsync();

        try
        {
            return await _windowService.FocusWindowAsync(_gameProcess!.MainWindowHandle);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while focusing overlay");
            return false;
        }
        finally
        {
            _focusLock.Release();
        }
    }

    #endregion

    #region Private methods

    private void Initialize()
    {
        Task.Run(FindGameProcess);
    }

    private async Task FindGameProcess()
    {
        if (!await _gameProcessLock.WaitAsync(0)) return;

        try
        {
            _gameProcess = null;
            var processFound = false;

            while (!processFound)
            {
                var configuration = await _appConfigurationService.GetConfigurationAsync();

                foreach (var processName in configuration.Game.ProcessNames)
                {
                    foreach (var process in Process.GetProcessesByName(processName))
                    {
                        if (process.HasExited) continue;

                        _gameProcess = process;
                        _gameProcess.Exited += (_, _) => Initialize();

                        _logger.LogDebug(
                            "Game process found: {ProcessName} ({ProcessId})",
                            process.ProcessName,
                            process.Id
                        );
                        processFound = true;
                        break;
                    }

                    if (processFound) break;
                }

                await Task.Delay(configuration.Game.WaitTimeIfNotFound);
            }
        }
        finally
        {
            _gameProcessLock.Release();
        }
    }

    #endregion
}