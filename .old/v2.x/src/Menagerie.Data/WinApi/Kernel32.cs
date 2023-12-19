using System.Runtime.InteropServices;
using System.Text;

namespace Menagerie.Data.WinApi;

public static class Kernel32
{
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool QueryFullProcessImageNameW(IntPtr hProcess, int flags, StringBuilder text,
        ref int count);

    [DllImport("kernel32.dll")]
    public static extern int GetLastError();
}