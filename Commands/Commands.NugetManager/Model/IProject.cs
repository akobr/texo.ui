using System.Collections.Immutable;

namespace BeaverSoft.Texo.Commands.NugetManager.Model
{
    public interface IProject
    {
        string Name { get; }

        string Path { get; }

        IImmutableDictionary<string, IPackage> Packages { get; }
    }
}
