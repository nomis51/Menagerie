using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Menagerie.Data.WinApi;
using Menagerie.Shared;
using Menagerie.Shared.Abstractions;
using Serilog;

namespace Menagerie.Data.Services;

public class GameProcessService : IService
{
    #region Constants

    private readonly List<string> _poeProcesses = Environment.GetEnvironmentVariable("ENV") == "dev"
        ? new List<string>
        {
            "notepad"
        }
        : new List<string>
        {
            "PathOfExile",
            "PathOfExile_Steam",
            "PathOfExile_x64",
            "PathOfExile_x64Steam",
            "PathOfExileSteam",
        };

    #endregion

    #region Members

    private int _processId = -1;
    private IntPtr _gameWindowHandle = IntPtr.Zero;

    #endregion


    #region Public methods

    public void Initialize()
    {
    }

    public Task Start()
    {
        return Task.Run(FindProcess);
    }

    public bool IsGameWindow(IntPtr hwnd)
    {
        if (_gameWindowHandle == IntPtr.Zero) return false;
        return _gameWindowHandle == hwnd;
    }

    #endregion

    #region Private methods

    private void FindProcess()
    {
        while (true)
        {
            foreach (var process in from poeProcess in _poeProcesses
                     select Process.GetProcessesByName(poeProcess)
                     into processes
                     where processes.Length != 0
                     select processes.FirstOrDefault()
                     into process
                     where process is not null && !process.HasExited
                     select process)
            {
                if (!FindLogFile(process)) continue;

                _processId = process.Id;
                _gameWindowHandle = process.MainWindowHandle;
                WatchProcess();
                AppDataService.Instance.GameProcessFound(_processId);
                AppDataService.Instance.IoHookProcess(_processId);

                return;
            }

            Task.Delay(5000).Wait();
        }
    }

    private Task WatchProcess()
    {
        return Task.Run(() =>
        {
            while (true)
            {
                try
                {
                    var process = Process.GetProcessById(_processId);
                    if (process.HasExited) break;

                    Thread.Sleep(5000);
                }
                catch (Exception e)
                {
                    Log.Information("Game process not running or exited: {Message}", e.Message);
                    break;
                }
            }

            Task.Run(FindProcess);
        });
    }

    private bool FindLogFile(Process process)
    {
        string clientFilePath;

        try
        {
            // 64 bits
            Log.Information("Trying to get 64bits location");

            if (process.MainModule == null) return false;
            Log.Information($"PoE location {process.MainModule.FileName}");

            if (process.MainModule.FileName == null) return false;
            clientFilePath =
                $"{process.MainModule.FileName[..process.MainModule.FileName.LastIndexOf("\\", StringComparison.Ordinal)]}\\logs\\Client.txt";

            if (!File.Exists(clientFilePath)) return false;
            Log.Information($"64bits Client file location: {clientFilePath}");

            AppDataService.Instance.ClientFileFound(clientFilePath);
            return true;
        }
        catch (Win32Exception)
        {
            try
            {
                //32 bits
                Log.Information("Failed trying to get 64bits location");
                Log.Information("Trying to get 32bits location");
                var size = 1024;
                var filename = new StringBuilder(size);
                var result = Kernel32.QueryFullProcessImageNameW(process.Handle, 0, filename, ref size);

                if (!result || size == 0)
                {
                    Log.Error($"Error while reading executable file path. Returned size was {size}");
                    // See https://docs.microsoft.com/en-us/windows/win32/debug/system-error-codes
                    var lastError = Kernel32.GetLastError();
                    throw new Exception($"GetLastError() = {lastError}");
                }

                Log.Information($"PoE location {filename}");
                clientFilePath =
                    $"{filename.ToString()[..filename.ToString().LastIndexOf("\\", StringComparison.Ordinal)]}\\logs\\Client.txt";
                Log.Information($"32bits Client file location: {clientFilePath}");

                if (!File.Exists(clientFilePath)) return false;

                AppDataService.Instance.ClientFileFound(clientFilePath);
                return true;
            }
            catch (Win32Exception)
            {
                Log.Information("Failed trying to get 32bits location");
            }
            catch (Exception e)
            {
                Log.Error("Error while getting 32bits PoE process location", e);
            }
        }
        catch (Exception e)
        {
            Log.Error("Error while getting 64bits PoE process location", e);
        }

        return false;
    }

    #endregion
}