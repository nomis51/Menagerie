using System.Diagnostics;
using Serilog;

namespace Menagerie.Shared.Helpers;

public class ProcessHelper
{
     #region Constants

    private static readonly IEnumerable<string> UnexpectedProcessesName = new[]
    {
        "menagerie",
    };

    #endregion

    #region Members

    private static readonly List<Process> Processes = [];

    #endregion

    #region Public methods

    public static Process SpawnProcess(ProcessStartInfo startInfo, string stdoutLogFilePath = "")
    {
        Log.Debug("Spawning process {Name} {Args}", startInfo.FileName, startInfo.Arguments);

        var process = new Process();
        process.StartInfo = startInfo;
        if (!string.IsNullOrEmpty(stdoutLogFilePath))
        {
            process.OutputDataReceived += (_, args) => File.AppendAllText(stdoutLogFilePath, args.Data);
        }

        process.Start();
        Processes.Add(process);
        return process;
    }

    public static Task CleanUnexpectedProcesses()
    {
        return Task.Run(() =>
        {
            var processes = Process.GetProcesses();
            var current = Process.GetCurrentProcess();
            var currentProcesses = Processes.Select(p => p.Id)
                .ToList();

            foreach (var process in processes)
            {
                try
                {
                    if (!UnexpectedProcessesName.Contains(process.ProcessName.ToLower()) || process.Id == current.Id || currentProcesses.Contains(process.Id)) continue;
                    Log.Information("Unexpected process cleaned PID {pid} {Name}", process.Id, process.MainModule?.FileName ?? "Unknown file name");
                    process.Kill();
                }
                catch (Exception e)
                {
                    Log.Warning("Unable to clear process {Id} {Name} properly: {Message}", process.Id, process.MainModule?.FileName ?? "Unknown file name", e.Message);
                }
            }
        });
    }

    public static void OnExit()
    {
        Log.Information("Disposing processes...");
        Dispose();
    }

    #endregion

    #region Private methods

    private static void Dispose()
    {
        Processes.ForEach(p =>
        {
            try
            {
                if (p.HasExited) return;
                p.StandardInput.WriteLine("\x3");
                p.WaitForExit(1000);

                if (p.HasExited) return;
                p.StandardInput.Close();
                p.WaitForExit(1000);

                if (p.HasExited) return;
                p.Kill();
            }
            catch (Exception)
            {
                // ignored
            }
        });
    }

    #endregion
}