using Menagerie.Core.Windows.Helpers.Abstractions;
using Microsoft.Extensions.Logging;

namespace Menagerie.Core.Windows;

public class WindowsPlatformDependencyResolver
{
    #region Props

    public ILogger<WindowsPlatformCapabilities> Logger { get; }
    public IClipboardHelper ClipboardHelper { get; }

    #endregion

    #region Constructors

    public WindowsPlatformDependencyResolver(
        ILogger<WindowsPlatformCapabilities> logger,
        IClipboardHelper clipboardHelper
    )
    {
        Logger = logger;
        ClipboardHelper = clipboardHelper;
    }

    #endregion
}