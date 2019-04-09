using System.Collections.Immutable;

namespace BeaverSoft.Texo.Commands.NugetManager.Model
{
    public class PackageInfo : IPackageInfo
    {
        public PackageInfo()
        {
            // no member
        }

        public PackageInfo(string packageId, ImmutableSortedSet<string> versions)
        {
            Id = packageId;
            Versions = versions;
        }

        public string Id { get; set; }

        public ImmutableSortedSet<string> Versions { get; set; }
    }
}
