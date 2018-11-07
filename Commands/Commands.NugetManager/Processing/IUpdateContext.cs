using BeaverSoft.Texo.Commands.NugetManager.Model.Packages;
using BeaverSoft.Texo.Commands.NugetManager.Model.Projects;

namespace BeaverSoft.Texo.Commands.NugetManager.Processing
{
    public interface IUpdateContext
    {
        IProject Project { get; }

        IPackage Package { get; }

        object Content { get; }
    }
}
