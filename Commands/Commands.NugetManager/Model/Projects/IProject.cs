using System.Collections.Immutable;
using BeaverSoft.Texo.Commands.NugetManager.Model.Packages;

namespace BeaverSoft.Texo.Commands.NugetManager.Model.Projects
{
    public interface IProject
    {
        string Name { get; }

        string Path { get; }

        IImmutableDictionary<string, IPackage> Packages { get; }
    }
}
