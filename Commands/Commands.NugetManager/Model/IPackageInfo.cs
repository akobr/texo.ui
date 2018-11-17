using System.Collections.Immutable;

namespace BeaverSoft.Texo.Commands.NugetManager.Model
{
    public interface IPackageInfo
    {
        string Id { get; }

        ImmutableSortedSet<string> Versions { get; }
    }
}
