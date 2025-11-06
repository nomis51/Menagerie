using System.Diagnostics;
using System.IO.Abstractions;
using Menagerie.Core.Linux.Platforms;
using Menagerie.Core.Shared.Abstractions;

namespace Menagerie.Core.Linux;

public class LinuxPlatformCapabilities : IPlatformCapabilities
{
    #region Members

    private readonly IPlatformCapabilities _platformCapabilities;
    private readonly IFileSystem _fileSystem;

    #endregion

    #region Constructors

    public LinuxPlatformCapabilities(LinuxPlatformDependencyResolver dependencyResolver)
    {
        _fileSystem = dependencyResolver.FileSystem;
        _platformCapabilities = GetPlatformCapabilities(dependencyResolver);
    }

    #endregion

    #region Public methods

    public Task<bool> FocusWindowAsync(Process process)
    {
        return _platformCapabilities.FocusWindowAsync(process);
    }

    public Task<bool> SetClipboardTextAsync(string text)
    {
        return _platformCapabilities.SetClipboardTextAsync(text);
    }

    public Task<string?> GetClipboardTextAsync()
    {
        return _platformCapabilities.GetClipboardTextAsync();
    }

    public Task<bool> ResetClipboardTextAsync()
    {
        return _platformCapabilities.ResetClipboardTextAsync();
    }

    public string? GetGameFolder(Process gameProcess)
    {
        // We assume it's Steam. Most likely running under Proton or Proton-GE.
        // So we can't do like on Windows and look at the process main module, since it's wine
        var folderPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".local",
            "share",
            "Steam",
            "steamapps",
            "common",
            "Path of Exile"
        );
        return _fileSystem.Directory.Exists(folderPath) ? folderPath : null;
    }

    public Task<bool> SendKeyboardPasteAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> SendKeyboardEnterAsync()
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Private methods

    private IPlatformCapabilities GetPlatformCapabilities(LinuxPlatformDependencyResolver dependencyResolver)
    {
        var sessionType = Environment.GetEnvironmentVariable("XDG_SESSION_TYPE");
        return sessionType switch
        {
            "wayland" => new WaylandPlatformCapabilities(dependencyResolver.WaylandPlatformDependencyResolver),
            "x11" => new X11PlatformCapabilities(),
            _ => throw new PlatformNotSupportedException("Unknown session type")
        };
    }

    #endregion
}