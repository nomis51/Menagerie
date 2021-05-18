using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Menagerie.Core.Abstractions;
using Menagerie.Core.Enums;
using Menagerie.Core.Extensions;

namespace Menagerie.Core.Services {
    public class PoeWindowService : IService {
        #region WinAPI
        [DllImport("user32.dll")]
        public static extern int SetForegroundWindow(int hwnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);

        [DllImport("psapi.dll")]
        static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, [In][MarshalAs(UnmanagedType.U4)] int nSize);

        [DllImport("psapi.dll")]
        static extern uint GetProcessImageFileNameA(IntPtr hProcess, [Out] StringBuilder lpBaseName, [In][MarshalAs(UnmanagedType.U4)] int nSize);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool QueryFullProcessImageNameW(IntPtr hProcess, int flags, StringBuilder text, ref int count);

        [DllImport("kernel32.dll")]
        static extern int GetLastError();

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();
        #endregion

        #region Constants
        private static readonly ILog log = LogManager.GetLogger(typeof(PoeWindowService));
        private readonly List<string> PoeProcesses = new List<string>() {
            "PathOfExile_x64",
            "PathOfExile_x64Steam"
        };
        #endregion

        #region Props
        private string _clientFilePath;
        public string ClientFilePath {
            get {
                return this._clientFilePath;
            }
        }

        public int ProcessId {
            get {
                return Process == null ? -1 : Process.Id;
            }
        }

        public bool Focused {
            get {
                return Process == null ? false : GetForegroundWindow() == Process.MainWindowHandle;
            }
        }
        #endregion

        #region Members
        private Process Process;
        #endregion

        #region Constructors
        public PoeWindowService() {
            log.Trace("Initializing PoeWindowService");
        }
        #endregion

        #region Private methods
        private bool ClientFileExists() {
            log.Trace("Verify client file exists");
            return this._clientFilePath != null && Directory.Exists(this._clientFilePath.Substring(0, this._clientFilePath.LastIndexOf("\\"))) && File.Exists(this._clientFilePath);
        }

        private async void FindPoeProcess() {
            log.Trace("Looking for PoE process");
            while (!ClientFileExists() || Process == null) {
                Process[] processes = Process.GetProcesses();

                foreach (var proc in processes) {
                    if (PoeProcesses.Contains(proc.ProcessName) && !proc.HasExited) {
                        log.Trace($"PoE process found");
                        this.Process = proc;

                        bool clientFileFound = false;

                        try {
                            // 64 bits
                            log.Trace("Trying to get 64bits location");
                            log.Trace($"PoE location {proc.MainModule.FileName}");
                            this._clientFilePath = $"{proc.MainModule.FileName.Substring(0, proc.MainModule.FileName.LastIndexOf("\\"))}\\logs\\Client.txt";
                            log.Trace($"64bits Client file location: {_clientFilePath}");
                            clientFileFound = true;
                        } catch (Win32Exception) {
                            try {
                                //32 bits
                                log.Trace("Failed trying to get 64bits location");
                                log.Trace("Trying to get 32bits location");
                                int size = 1024;
                                StringBuilder filename = new StringBuilder(size);
                                var result = QueryFullProcessImageNameW(proc.Handle, 0, filename, ref size);

                                if (!result || size == 0) {
                                    log.Error($"Error while reading executable file path. Returned size was {size}");
                                    // See https://docs.microsoft.com/en-us/windows/win32/debug/system-error-codes
                                    var lastError = GetLastError();
                                    throw new Exception($"GetLastError() = {lastError}");
                                }

                                log.Trace($"PoE location {filename}");
                                _clientFilePath = $"{filename.ToString().Substring(0, filename.ToString().LastIndexOf("\\"))}\\logs\\Client.txt";
                                log.Trace($"32bits Client file location: {_clientFilePath}");
                                clientFileFound = true;
                            } catch (Win32Exception) {
                                log.Trace("Failed trying to get 32bits location");
                            } catch (Exception e) {
                                log.Error("Error while getting 32bits PoE process location", e);
                            }
                        } catch (Exception e) {
                            log.Error("Error while getting 64bits PoE process location", e);
                        }

                        if (!clientFileFound) {
                            Process = null;
                            _clientFilePath = null;
                            break;
                        }

                        AppService.Instance.PoeWindowReady();
                        Focus();

                        AppService.Instance.ClientFileReady();
                    }
                }

                if (!ClientFileExists() || Process == null) {
                    log.Trace("Unable to find any known PoE processes");
                    log.Trace("Waiting 5 seconds before retry");
                    await Task.Delay(5000);
                }
            }

            log.Trace("PoE process and client file found");
        }
        #endregion

        #region Public methods
        public bool EnsurePoeWindowAlive() {
            log.Trace("Ensuring Poe window alive");
            if (Process.HasExited) {
                Process = null;
                Task.Run(() => FindPoeProcess());
                return false;
            }

            return true;
        }

        private bool IsGameWindowFocused() {
            IntPtr activeHandle = GetForegroundWindow();
            return activeHandle == Process.MainWindowHandle;
        }

        public bool Focus() {
            log.Trace("Focusing PoE");
            if (Process == null) {
                return false;
            }

            if (Focused) {
                return true;
            }

            int i = 0;

            while (!IsGameWindowFocused() && i < 3) {
                ShowWindow(Process.MainWindowHandle, ShowWindowEnum.Show);
                SetForegroundWindow((int)Process.MainWindowHandle);

                Thread.Sleep(200);
                ++i;
            }

            return IsGameWindowFocused();
        }

        public void Start() {
            log.Trace("Starting PoeWindowService");
            FindPoeProcess();
        }
        #endregion
    }
}
