using log4net;
using Menagerie.Core.Abstractions;
using System;
using Winook;
using Menagerie.Core.Extensions;
using System.Runtime.InteropServices;
using Menagerie.Core.Helpers;
using Desktop.Robot;

namespace Menagerie.Core.Services
{
    public class KeyboardService : IService
    {
        #region Constants
        private static readonly ILog log = LogManager.GetLogger(typeof(KeyboardService));
        #endregion

        #region Members
        private LinuxKeyboardHook _linuxKeyboardHook;
        private KeyboardHook _windowsKeyboardHook;

        private Robot _robot;
        #endregion

        #region Constructors
        public KeyboardService()
        {
            log.Trace("Initializing KeyboardService");
            _robot = new Robot();
        }
        #endregion

        #region Public methods
        public void HookProcess(int processId)
        {
            log.Trace($"Hooking process {processId}");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (processId != 0)
                {
                    if (_windowsKeyboardHook != null)
                    {
                        _windowsKeyboardHook.MessageReceived -= WindowsKeyboardHook_MessageReceived;
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
                        log.Error($"Error while hooking process {processId}", e);
                    }
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                _linuxKeyboardHook = new LinuxKeyboardHook();
                _linuxKeyboardHook.NewKeyboard += LinuxKeyboardHook_NewKeyboard;
                _linuxKeyboardHook.HookKeyboard();
            }
        }

        private void LinuxKeyboardHook_NewKeyboard()
        {

        }

        private void WindowsKeyboardHook_MessageReceived(object sender, KeyboardMessageEventArgs e)
        {
            AppService.Instance.HandleKeyboardInput(e);
        }

        public void KeyPress(Key key)
        {
            log.Trace($"Sending key press {(int)key}");
            _robot.KeyPress(key);
        }

        public void KeyUp(Key key)
        {
            log.Trace($"Sending key up {(int)key}");
            _robot.KeyUp(key);
        }

        public void KeyDown(Key key)
        {
            log.Trace($"Sending key down {(int)key}");
            _robot.KeyDown(key);
        }

        public void ClearSpecialKeys()
        {
            log.Trace($"Clearing special keys");
            KeyUp(Key.Control);
            KeyUp(Key.Shift);
        }

        public void ModifiedKeyStroke(Key modifier, Key key)
        {
            log.Trace($"Sending modified key strokes for {(int)key} with {(int)modifier}");
            _robot.KeyDown(modifier);
            _robot.KeyPress(key);
            _robot.KeyUp(modifier);
        }

        public void SendEnter()
        {
            log.Trace("Sending Enter key press");
            _robot.KeyPress(Key.Enter);
        }

        public void SendCtrlA()
        {
            log.Trace("Sending Ctrl + A");
            ModifiedKeyStroke(Key.Control, Key.A);
        }

        public void SendBackspace()
        {
            log.Trace("Sending Backspace key press");
            _robot.KeyPress(Key.Backspace);
        }

        public void SendEscape()
        {
            log.Trace("Sending Escape key press");
            _robot.KeyPress(Key.Esc);
        }

        public void SendCtrlV()
        {
            log.Trace("Sending Ctrl + V");
            ModifiedKeyStroke(Key.Control, Key.V);
        }

        public void SendCtrlC()
        {
            log.Trace("Sending Ctrl + C");
            ModifiedKeyStroke(Key.Control, Key.C);
        }

        public void Start()
        {
            log.Trace("Starting KeyboardService");
        }
        #endregion
    }
}
