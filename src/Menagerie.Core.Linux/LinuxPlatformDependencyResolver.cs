using Menagerie.Core.Linux.Platforms;

namespace Menagerie.Core.Linux;

public class LinuxPlatformDependencyResolver
{
    public LinuxPlatformDependencyResolver(WaylandPlatformDependencyResolver waylandPlatformDependencyResolver)
    {
        WaylandPlatformDependencyResolver = waylandPlatformDependencyResolver;
    }

    #region Props

    public WaylandPlatformDependencyResolver WaylandPlatformDependencyResolver { get; }

    #endregion

    #region Constructors

    #endregion
}