using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Toucan.Core {
    public class PoeWindowHandler : Handler {
        private static bool DEBUG = true;

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
        #endregion

        #region Events
        public delegate void PoeWindowReady();
        public event PoeWindowReady OnPoeWindowReady;

        public delegate void ClientFileReady();
        public event ClientFileReady OnClientFileReady;
        #endregion

        #region Singleton
        private static PoeWindowHandler _instance;
        public static PoeWindowHandler Instance {
            get {
                if (_instance == null) {
                    _instance = new PoeWindowHandler();
                }

                return _instance;
            }
        }
        #endregion

        private readonly List<string> PoeProcesses = new List<string>() {
            "PathOfExile",
            "PathOfExile_x64",
            "notepad"
        };

        private string _clientFilePath;
        public string ClientFilePath {
            get {
                return this._clientFilePath;
            }
        }

        private Process Process;

        private PoeWindowHandler() { }

        public override void Start() {
            FindPoeProcess();
            Ready = true;
        }

        private bool ClientFileExists() {
            return this._clientFilePath != null && Directory.Exists(this._clientFilePath.Substring(0, this._clientFilePath.LastIndexOf("\\"))) && File.Exists(this._clientFilePath);
        }

        private async void FindPoeProcess() {
            while (!ClientFileExists()) {
                Process[] processes = Process.GetProcesses();

                foreach (var proc in processes) {
                    if (PoeProcesses.Contains(proc.ProcessName)) {
                        this.Process = proc;

                        OnPoeWindowReady();

                        if (DEBUG) {
                            _clientFilePath = @"C:\Path of Exile\logs\Client.txt";
                        } else {
                            try {
                                // 64 bits
                                this._clientFilePath = $"{proc.MainModule.FileName.Substring(0, proc.MainModule.FileName.LastIndexOf("\\"))}\\logs\\Client.txt";
                            } catch (Win32Exception) {
                                //32 bits
                                StringBuilder filename = new StringBuilder(1024);
                                GetModuleFileNameEx(proc.Handle, IntPtr.Zero, filename, 2014);
                                _clientFilePath = $"{filename.ToString().Substring(0, filename.ToString().LastIndexOf("\\"))}\\logs\\Client.txt";
                            }
                        }

                        OnClientFileReady();
                        break;
                    }
                }

                await Task.Delay(5000);
            }
        }

        public void Focus() {
            if (Process == null) {
                return;
            }

            ShowWindow(Process.MainWindowHandle, ShowWindowEnum.Restore);

            SetForegroundWindow((int)Process.MainWindowHandle);
        }

        private enum ShowWindowEnum {
            Hide = 0,
            ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3,
            Maximize = 3, ShowNormalNoActivate = 4, Show = 5,
            Minimize = 6, ShowMinNoActivate = 7, ShowNoActivate = 8,
            Restore = 9, ShowDefault = 10, ForceMinimized = 11
        };
    }
}
