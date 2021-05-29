using log4net;
using Menagerie.Core.Abstractions;
using System;
using System.Drawing;
using Winook;
using Menagerie.Core.Extensions;
using System.Runtime.InteropServices;
using Desktop.Robot;

namespace Menagerie.Core.Services
{
    public class KeyboardService : IService
    {
        #region Constants

        private static readonly ILog Log = LogManager.GetLogger(typeof(KeyboardService));

        #endregion

        #region Members

        private KeyboardHook _windowsKeyboardHook;
        private MouseHook _windowsMouseHook;

        private readonly Robot _robot;

        #endregion

        #region Constructors

        public KeyboardService()
        {
            Log.Trace("Initializing KeyboardService");
            _robot = new Robot();
        }

        #endregion

        #region Public methods

        public void HookProcess(int processId)
        {
            Log.Trace($"Hooking process {processId}");

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;
            if (processId == 0) return;
            if (_windowsKeyboardHook != null)
            {
                _windowsKeyboardHook.MessageReceived -= WindowsKeyboardHook_MessageReceived;
                _windowsKeyboardHook.Dispose();
                _windowsKeyboardHook = null;
            }

            if (_windowsMouseHook != null)
            {
                _windowsMouseHook.MouseMove -= WindowsMouseHookOnMouseMove;
                _windowsMouseHook.Dispose();
                _windowsMouseHook = null;
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
            
            try
            {
                _windowsMouseHook = new MouseHook(processId);
                _windowsMouseHook.MouseMove += WindowsMouseHookOnMouseMove;
                _windowsMouseHook.InstallAsync().Wait();
            }
            catch (Exception e)
            {
                Log.Error($"Error while mouse hooking process {processId}", e);
            }
        }

        private void WindowsMouseHookOnMouseMove(object? sender, MouseMessageEventArgs e)
        {
                AppService.Instance.MouseMoved();
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
            Log.Trace($"Sending key press {(int) key}");
            _robot.KeyPress(key);
        }

        public void KeyUp(Key key)
        {
            Log.Trace($"Sending key up {(int) key}");
            _robot.KeyUp(key);
        }

        public void KeyDown(Key key)
        {
            Log.Trace($"Sending key down {(int) key}");
            _robot.KeyDown(key);
        }

        public void ClearSpecialKeys()
        {
            Log.Trace($"Clearing special keys");
            KeyUp(Key.Control);
            KeyUp(Key.Shift);
        }

        public void ModifiedKeyStroke(Key modifier, Key key)
        {
            Log.Trace($"Sending modified key strokes for {(int) key} with {(int) modifier}");
            _robot.KeyDown(modifier);
            _robot.KeyPress(key);
            _robot.KeyUp(modifier);
        }

        public void SendEnter()
        {
            Log.Trace("Sending Enter key press");
            _robot.KeyPress(Key.Enter);
        }

        public void SendCtrlA()
        {
            Log.Trace("Sending Ctrl + A");
            ModifiedKeyStroke(Key.Control, Key.A);
        }

        public void SendBackspace()
        {
            Log.Trace("Sending Backspace key press");
            _robot.KeyPress(Key.Backspace);
        }

        public void SendEscape()
        {
            Log.Trace("Sending Escape key press");
            _robot.KeyPress(Key.Esc);
        }

        public void SendCtrlV()
        {
            Log.Trace("Sending Ctrl + V");
            ModifiedKeyStroke(Key.Control, Key.V);
        }

        public void SendCtrlC()
        {
            Log.Trace("Sending Ctrl + C");
            ModifiedKeyStroke(Key.Control, Key.C);
        }

        public void Start()
        {
            Log.Trace("Starting KeyboardService");
        }

        #endregion
    }
}