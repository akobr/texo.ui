using BeaverSoft.Texo.Commands.NugetManager.Packages;
using BeaverSoft.Texo.Commands.NugetManager.Projects;

namespace BeaverSoft.Texo.Commands.NugetManager.Processing
{
    public interface IUpdateContext
    {
        IProject Project { get; }

        IPackage Package { get; }

        object Content { get; }
    }
}
