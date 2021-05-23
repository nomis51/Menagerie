using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Menagerie.Core.Helpers
{
    public class LinuxKeyboardHook
    {
        #region Events

        public delegate void NewKeyboardEvent();

        public event NewKeyboardEvent NewKeyboard;

        #endregion

        #region Members

        private bool _isHooked = false;

        #endregion


        public LinuxKeyboardHook()
        {
        }

        private void OnNewKeyboardEvent()
        {
            NewKeyboard?.Invoke();
        }

        public void HookKeyboard()
        {
            _isHooked = true;
            Task.Run(() => Listen());
        }

        public void UnHookKeyboard()
        {
            _isHooked = false;
        }

        private void Listen()
        {
            string readMessage = "";
            try
            {
                FileStream stream = new FileStream("/dev/input/event0", FileMode.Open, FileAccess.Read,
                    FileShare.ReadWrite);
                byte[] buffer = new byte[24];

                while (_isHooked)
                {
                    stream.Read(buffer, 0, buffer.Length);

                    // parse timeval (8 bytes)
                    int offset = 8;
                    short type = BitConverter.ToInt16(new byte[] {buffer[offset], buffer[++offset]}, 0);
                    short code = BitConverter.ToInt16(new byte[] {buffer[++offset], buffer[++offset]}, 0);
                    int value = BitConverter.ToInt32(
                        new byte[] {buffer[++offset], buffer[++offset], buffer[++offset], buffer[++offset]}, 0);

                    if (value == 1 && code != 28)
                    {
                        var key = (((KEY_CODE) code).ToString()).Replace("KEY_", "");
                        key = key.Replace("MINUS", "-");
                        key = key.Replace("EQUAL", "=");
                        key = key.Replace("SEMICOLON", ";");
                        key = key.Replace("COMMA", ",");
                        key = key.Replace("SLASH", "/");

                        readMessage += key;
                    }

                    if (code == 28)
                    {
                        ParseMessage(readMessage);
                        readMessage = "";
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void ParseMessage(string message)
        {
            OnNewKeyboardEvent();
        }
    }

    public enum KEY_CODE
    {
        KEY_1 = 2,
        KEY_2,
        KEY_3,
        KEY_4,
        KEY_5,
        KEY_6,
        KEY_7,
        KEY_8,
        KEY_9,
        KEY_0,
        KEY_MINUS,
        KEY_EQUAL,
        KEY_BACKSPACE,
        KEY_TAB,
        KEY_Q,
        KEY_W,
        KEY_E,
        KEY_R,
        KEY_T,
        KEY_Y,
        KEY_U,
        KEY_I,
        KEY_O,
        KEY_P,
        KEY_LEFTBRACE,
        KEY_RIGHTBRACE,
        KEY_ENTER,
        KEY_LEFTCTRL,
        KEY_A,
        KEY_S,
        KEY_D,
        KEY_F,
        KEY_G,
        KEY_H,
        KEY_J,
        KEY_K,
        KEY_L,
        KEY_SEMICOLON,
        KEY_APOSTROPHE,
        KEY_GRAVE,
        KEY_LEFTSHIFT,
        KEY_BACKSLASH,
        KEY_Z,
        KEY_X,
        KEY_C,
        KEY_V,
        KEY_B,
        KEY_N,
        KEY_M,
        KEY_COMMA,
        KEY_DOT,
        KEY_SLASH,
        KEY_RIGHTSHIFT,
        KEY_KPASTERISK,
        KEY_LEFTALT,
        KEY_SPACE,
        KEY_CAPSLOCK,
        KEY_F1,
        KEY_F2,
        KEY_F3,
        KEY_F4,
        KEY_F5,
        KEY_F6,
        KEY_F7,
        KEY_F8,
        KEY_F9,
        KEY_F10,
        KEY_NUMLOCK,
        KEY_SCROLLLOCK,
        KEY_KP7,
        KEY_KP8,
        KEY_KP9,
        KEY_KPMINUS,
        KEY_KP4,
        KEY_KP5,
        KEY_KP6,
        KEY_KPPLUS,
        KEY_KP1,
        KEY_KP2,
        KEY_KP3,
        KEY_KP0,
        KEY_KPDOT
    }
}