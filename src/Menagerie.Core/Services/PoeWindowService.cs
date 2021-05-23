using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Menagerie.Core.Abstractions;
using Menagerie.Core.Enums;
using Menagerie.Core.Extensions;

namespace Menagerie.Core.Services
{
    public class PoeWindowService : IService
    {
        #region WinAPI

        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(int hwnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool QueryFullProcessImageNameW(IntPtr hProcess, int flags, StringBuilder text,
            ref int count);

        [DllImport("kernel32.dll")]
        private static extern int GetLastError();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        #endregion

        #region Constants

        private static readonly ILog Log = LogManager.GetLogger(typeof(PoeWindowService));

        private readonly List<string> _poeProcesses = new List<string>()
        {
            "PathOfExile_x64",
            "PathOfExile_x64Steam"
        };

        #endregion

        #region Props

        public string ClientFilePath { get; private set; }

        public int ProcessId => _process?.Id ?? -1;

        public bool Focused => _process != null && GetForegroundWindow() == _process.MainWindowHandle;

        #endregion

        #region Members

        private Process _process;

        #endregion

        #region Constructors

        public PoeWindowService()
        {
            Log.Trace("Initializing PoeWindowService");
        }

        #endregion

        #region Private methods

        private bool ClientFileExists()
        {
            Log.Trace("Verify client file exists");
            return ClientFilePath != null &&
                   Directory.Exists(ClientFilePath[..ClientFilePath.LastIndexOf("\\", StringComparison.Ordinal)]) &&
                   File.Exists(ClientFilePath);
        }

        private async void FindPoeProcess()
        {
            Log.Trace("Looking for PoE process");
            while (!ClientFileExists() || _process == null)
            {
                var processes = Process.GetProcesses();

                foreach (var proc in processes)
                {
                    if (!_poeProcesses.Contains(proc.ProcessName) || proc.HasExited) continue;
                    Log.Trace($"PoE process found");
                    _process = proc;

                    var clientFileFound = false;

                    try
                    {
                        // 64 bits
                        Log.Trace("Trying to get 64bits location");
                        if (proc.MainModule != null)
                        {
                            Log.Trace($"PoE location {proc.MainModule.FileName}");
                            if (proc.MainModule.FileName != null)
                            {
                                ClientFilePath =
                                    $"{proc.MainModule.FileName[..proc.MainModule.FileName.LastIndexOf("\\", StringComparison.Ordinal)]}\\logs\\Client.txt";
                            }
                        }

                        Log.Trace($"64bits Client file location: {ClientFilePath}");
                        clientFileFound = true;
                    }
                    catch (Win32Exception)
                    {
                        try
                        {
                            //32 bits
                            Log.Trace("Failed trying to get 64bits location");
                            Log.Trace("Trying to get 32bits location");
                            var size = 1024;
                            var filename = new StringBuilder(size);
                            var result = QueryFullProcessImageNameW(proc.Handle, 0, filename, ref size);

                            if (!result || size == 0)
                            {
                                Log.Error($"Error while reading executable file path. Returned size was {size}");
                                // See https://docs.microsoft.com/en-us/windows/win32/debug/system-error-codes
                                var lastError = GetLastError();
                                throw new Exception($"GetLastError() = {lastError}");
                            }

                            Log.Trace($"PoE location {filename}");
                            ClientFilePath =
                                $"{filename.ToString()[..filename.ToString().LastIndexOf("\\", StringComparison.Ordinal)]}\\logs\\Client.txt";
                            Log.Trace($"32bits Client file location: {ClientFilePath}");
                            clientFileFound = true;
                        }
                        catch (Win32Exception)
                        {
                            Log.Trace("Failed trying to get 32bits location");
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

                    if (!clientFileFound)
                    {
                        _process = null;
                        ClientFilePath = null;
                        break;
                    }

                    AppService.Instance.PoeWindowReady();
                    Focus();

                    AppService.Instance.ClientFileReady();
                    break;
                }

                if (ClientFileExists() && _process != null) continue;
                Log.Trace("Unable to find any known PoE processes");
                Log.Trace("Waiting 5 seconds before retry");
                await Task.Delay(5000);
            }

            Log.Trace("PoE process and client file found");
        }

        #endregion

        #region Public methods

        public bool EnsurePoeWindowAlive()
        {
            Log.Trace("Ensuring Poe window alive");
            if (!_process.HasExited) return true;
            _process = null;
            Task.Run(FindPoeProcess);
            return false;
        }

        private bool IsGameWindowFocused()
        {
            var activeHandle = GetForegroundWindow();
            return activeHandle == _process.MainWindowHandle;
        }

        public bool Focus()
        {
            Log.Trace("Focusing PoE");
            if (_process == null)
            {
                return false;
            }

            if (Focused)
            {
                return true;
            }

            var i = 0;

            while (!IsGameWindowFocused() && i < 3)
            {
                ShowWindow(_process.MainWindowHandle, ShowWindowEnum.Show);
                // ReSharper disable once CA1806
                SetForegroundWindow((int) _process.MainWindowHandle);

                Thread.Sleep(200);
                ++i;
            }

            return IsGameWindowFocused();
        }

        public void Start()
        {
            Log.Trace("Starting PoeWindowService");
            FindPoeProcess();
        }

        #endregion
    }
}