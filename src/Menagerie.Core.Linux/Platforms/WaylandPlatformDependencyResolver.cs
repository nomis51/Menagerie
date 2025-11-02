using Menagerie.Core.Linux.Helpers.Abstractions;

namespace Menagerie.Core.Linux.Platforms;

public class WaylandPlatformDependencyResolver
{
    #region Props

    public IClipboardHelper ClipboardHelper { get; }

    #endregion

    #region Constructors

    public WaylandPlatformDependencyResolver(IClipboardHelper clipboardHelper)
    {
        ClipboardHelper = clipboardHelper;
    }

    #endregion
}