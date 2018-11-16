using System.Collections.Immutable;

namespace BeaverSoft.Texo.Commands.NugetManager.Model.Packages
{
    public interface IPackageInfo
    {
        string Id { get; }

        ImmutableSortedSet<string> Versions { get; }
    }
}
