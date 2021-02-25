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
                        log.Trace("PoE process found");
                        this.Process = proc;

                        AppService.Instance.PoeWindowReady();
                        Focus();

                        try {
                            // 64 bits
                            log.Trace("Trying to get 64bits location");
                            this._clientFilePath = $"{proc.MainModule.FileName.Substring(0, proc.MainModule.FileName.LastIndexOf("\\"))}\\logs\\Client.txt";
                            log.Trace($"32bits Client file location: {_clientFilePath}");
                        } catch (Win32Exception) {
                            //32 bits
                            log.Trace("Failed trying to get 64bits location");
                            log.Trace("Trying to get 32bits location");
                            StringBuilder filename = new StringBuilder(1024);
                            GetModuleFileNameEx(proc.Handle, IntPtr.Zero, filename, 2014);
                            _clientFilePath = $"{filename.ToString().Substring(0, filename.ToString().LastIndexOf("\\"))}\\logs\\Client.txt";
                            log.Trace($"64bits Client file location: {_clientFilePath}");
                        } catch (Exception e) {
                            log.Error("Error while getting PoE process location", e);
                        }

                        AppService.Instance.ClientFileReady();
                        break;
                    }
                }

                log.Trace("Unable to find any known PoE processes");
                log.Trace("Waiting 5 seconds before retry");
                await Task.Delay(5000);
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
