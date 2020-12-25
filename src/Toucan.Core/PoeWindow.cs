using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Toucan.Core {
    public class PoeWindow {
        [DllImport("user32.dll")]
        public static extern int SetForegroundWindow(int hwnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);

        [DllImport("psapi.dll")]
        static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, [In][MarshalAs(UnmanagedType.U4)] int nSize);

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

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

        private bool _clientFileReady = false;
        public bool ClientFileReady {
            get {
                return this._clientFileReady;
            }
        }

        private Process Process;

        public PoeWindow() {
            this.FindPoeProcess();

            if (this.ClientFileExists()) {
                this._clientFileReady = true;
            }
        }

        private bool ClientFileExists() {
            return this._clientFilePath != null && Directory.Exists(this._clientFilePath.Substring(0, this._clientFilePath.LastIndexOf("\\"))) && File.Exists(this._clientFilePath);
        }

        private void FindPoeProcess() {
            Process[] processes = Process.GetProcesses();

            string raw = "";
            processes.Select(p => p.ProcessName).ToList().ForEach(l => raw += l + "\n");

            File.AppendAllText("./toucan.log", raw + "\n");

            foreach (var proc in processes) {
                if (PoeProcesses.Contains(proc.ProcessName)) {
                    this.Process = proc;

                    File.AppendAllText("./toucan.log", "Poe process found\n");

                    try {
                        // 64 bits
                        this._clientFilePath = $"{proc.MainModule.FileName.Substring(0, proc.MainModule.FileName.LastIndexOf("\\"))}\\logs\\Client.txt";
                    } catch (Win32Exception) {
                        //32 bits
                        StringBuilder filename = new StringBuilder(1024);
                        GetModuleFileNameEx(proc.Handle, IntPtr.Zero, filename, 2014);
                        _clientFilePath = $"{filename.ToString().Substring(0, filename.ToString().LastIndexOf("\\"))}\\logs\\Client.txt";
                    }

                    File.AppendAllText("./toucan.log", $"Client path: {_clientFilePath}\n");
                    break;
                }
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
