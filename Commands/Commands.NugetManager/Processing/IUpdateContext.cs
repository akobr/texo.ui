using BeaverSoft.Texo.Commands.NugetManager.Model;

namespace BeaverSoft.Texo.Commands.NugetManager.Processing
{
    public interface IUpdateContext
    {
        IProject Project { get; }

        IPackage Package { get; }

        object Content { get; }
    }
}
