using System.IO.Abstractions;
using Menagerie.Core.Linux.Platforms;

namespace Menagerie.Core.Linux;

public class LinuxPlatformDependencyResolver
{
    #region Props

    public WaylandPlatformDependencyResolver WaylandPlatformDependencyResolver { get; }

    public IFileSystem FileSystem { get; }

    #endregion

    #region Constructors

    public LinuxPlatformDependencyResolver(
        WaylandPlatformDependencyResolver waylandPlatformDependencyResolver,
        IFileSystem fileSystem
    )
    {
        WaylandPlatformDependencyResolver = waylandPlatformDependencyResolver;
        FileSystem = fileSystem;
    }

    #endregion
}