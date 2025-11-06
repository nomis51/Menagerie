using System.Diagnostics;
using System.Runtime.InteropServices;
using Menagerie.Core.Linux;
using Menagerie.Core.Shared.Abstractions;
using Menagerie.Core.Windows;

namespace Menagerie.Core;

public class PlatformCapabilities : IPlatformCapabilities
{
    #region Members

    private readonly IPlatformCapabilities _platformCapabilities;
    private readonly WindowsPlatformDependencyResolver _windowsPlatformDependencyResolver;
    private readonly LinuxPlatformDependencyResolver _linuxPlatformDependencyResolver;

    #endregion

    #region Constructors

    public PlatformCapabilities(
        WindowsPlatformDependencyResolver windowsPlatformDependencyResolver,
        LinuxPlatformDependencyResolver linuxPlatformDependencyResolver
    )
    {
        _windowsPlatformDependencyResolver = windowsPlatformDependencyResolver;
        _linuxPlatformDependencyResolver = linuxPlatformDependencyResolver;

        _platformCapabilities = GetPlatformCapabilities();
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
        return _platformCapabilities.GetGameFolder(gameProcess);
    }

    public Task<bool> SendKeyboardPasteAsync()
    {
        return _platformCapabilities.SendKeyboardPasteAsync();
    }

    #endregion

    #region Private methods

    private IPlatformCapabilities GetPlatformCapabilities()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return new WindowsPlatformCapabilities(_windowsPlatformDependencyResolver);
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return new LinuxPlatformCapabilities(_linuxPlatformDependencyResolver);
        }

        throw new PlatformNotSupportedException($"Unsupported platform: {RuntimeInformation.OSDescription}");
    }

    #endregion
}