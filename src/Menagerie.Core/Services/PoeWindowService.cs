using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Menagerie.Core.Abstractions;
using Menagerie.Core.Enums;

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
        private static bool DEBUG = true;
        private readonly List<string> PoeProcesses = new List<string>() {
            "notepad",
            "PathOfExile",
            "PathOfExile_x64",
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
        }
        #endregion

        #region Private methods
        private bool ClientFileExists() {
            return this._clientFilePath != null && Directory.Exists(this._clientFilePath.Substring(0, this._clientFilePath.LastIndexOf("\\"))) && File.Exists(this._clientFilePath);
        }

        private async void FindPoeProcess() {
            while (!ClientFileExists()) {
                Process[] processes = Process.GetProcesses();

                foreach (var proc in processes) {
                    if (PoeProcesses.Contains(proc.ProcessName)) {
                        this.Process = proc;

                        AppService.Instance.PoeWindowReady();
                        Focus();

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

                        AppService.Instance.ClientFileReady();
                        break;
                    }
                }

                await Task.Delay(5000);
            }
        }
        #endregion

        #region Public methods
        public void Focus() {
            if (Process == null) {
                return;
            }

            if (Focused) {
                return;
            }

            ShowWindow(Process.MainWindowHandle, ShowWindowEnum.Restore);
            SetForegroundWindow((int)Process.MainWindowHandle);
        }

        public void Start() {
            FindPoeProcess();
        }
        #endregion
    }
}
