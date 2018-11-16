using System.Collections.Immutable;

namespace BeaverSoft.Texo.Commands.NugetManager.Model.Packages
{
    public interface IPackageInfo
    {
        string Id { get; }

        IImmutableSet<string> Versions { get; }
    }
}
