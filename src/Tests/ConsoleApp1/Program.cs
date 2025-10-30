using System.Diagnostics;
using Menagerie.Core.OS.Linux;
using Menagerie.Core.Services;

var windowService = new WindowService(new LinuxLibs());
var processes = Process.GetProcesses();
var process = Process.GetProcessesByName("PathOfExileStea")[0];
await windowService.FocusWindowAsync(process);