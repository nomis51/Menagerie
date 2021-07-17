using Menagerie.Core.Abstractions;
using System;
using System.Drawing;
using Winook;
using System.Runtime.InteropServices;
using Desktop.Robot;
using Serilog;

namespace Menagerie.Core.Services
{
    public class KeyboardService : IService
    {
        #region Members

        private Point _previousMousePosition = new Point(-1, -1);
        private KeyboardHook _windowsKeyboardHook;

        private readonly Robot _robot;

        #endregion

        #region Constructors

        public KeyboardService()
        {
            Log.Information("Initializing KeyboardService");
            _robot = new Robot();
        }

        #endregion

        #region Public methods

        public void HookProcess(int processId)
        {
            Log.Information($"Hooking process {processId}");

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;
            if (processId == 0) return;
            if (_windowsKeyboardHook != null)
            {
                _windowsKeyboardHook.MessageReceived -= WindowsKeyboardHook_MessageReceived;
                _windowsKeyboardHook.Dispose();
                _windowsKeyboardHook = null;
            }

            try
            {
                _windowsKeyboardHook = new KeyboardHook(processId);
                _windowsKeyboardHook.MessageReceived += WindowsKeyboardHook_MessageReceived;
                _windowsKeyboardHook.InstallAsync().Wait();
            }
            catch (Exception e)
            {
                Log.Error($"Error while hooking process {processId}", e);
            }
        }

        private void VerifyMouseMoved()
        {
            while (true)
            {
                var currentPosition = GetMousePosition();

                if (_previousMousePosition.X == -1 && _previousMousePosition.Y == -1)
                {
                    _previousMousePosition = currentPosition;
                }
                else if (_previousMousePosition.X != currentPosition.X || _previousMousePosition.Y != currentPosition.Y)
                {
                    AppService.Instance.MouseMoved();
                }
            }
        }

        private static void WindowsKeyboardHook_MessageReceived(object sender, KeyboardMessageEventArgs e)
        {
            AppService.Instance.HandleKeyboardInput(e);
        }

        public Point GetMousePosition()
        {
            return _robot.GetMousePosition();
        }

        public void KeyPress(Key key)
        {
            Log.Information($"Sending key press {(int) key}");
            _robot.KeyPress(key);
        }

        public void KeyUp(Key key)
        {
            Log.Information($"Sending key up {(int) key}");
            _robot.KeyUp(key);
        }

        public void KeyDown(Key key)
        {
            Log.Information($"Sending key down {(int) key}");
            _robot.KeyDown(key);
        }

        public void ClearSpecialKeys()
        {
            Log.Information($"Clearing special keys");
            KeyUp(Key.Control);
            KeyUp(Key.Shift);
        }

        public void ModifiedKeyStroke(Key modifier, Key key)
        {
            Log.Information($"Sending modified key strokes for {(int) key} with {(int) modifier}");
            _robot.KeyDown(modifier);
            _robot.KeyPress(key);
            _robot.KeyUp(modifier);
        }

        public void SendEnter()
        {
            Log.Information("Sending Enter key press");
            _robot.KeyPress(Key.Enter);
        }

        public void SendCtrlA()
        {
            Log.Information("Sending Ctrl + A");
            ModifiedKeyStroke(Key.Control, Key.A);
        }

        public void SendBackspace()
        {
            Log.Information("Sending Backspace key press");
            _robot.KeyPress(Key.Backspace);
        }

        public void SendEscape()
        {
            Log.Information("Sending Escape key press");
            _robot.KeyPress(Key.Esc);
        }

        public void SendCtrlV()
        {
            Log.Information("Sending Ctrl + V");
            ModifiedKeyStroke(Key.Control, Key.V);
        }

        public void SendCtrlC()
        {
            Log.Information("Sending Ctrl + C");
            ModifiedKeyStroke(Key.Control, Key.C);
        }

        public void Start()
        {
            Log.Information("Starting KeyboardService");
            //Task.Run(VerifyMouseMoved);
        }

        #endregion
    }
}