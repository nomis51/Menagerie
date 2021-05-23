using System;

namespace Menagerie.Test.LinuxHook
{
    class Program
    {
        static void Main(string[] args)
        {
            Menagerie.Core.Helpers.LinuxKeyboardHook lkh = new Core.Helpers.LinuxKeyboardHook();
            lkh.HookKeyboard();

            Console.ReadKey();
        }
    }
}
