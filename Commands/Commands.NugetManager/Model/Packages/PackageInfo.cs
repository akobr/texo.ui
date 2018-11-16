using System.Collections.Immutable;

namespace BeaverSoft.Texo.Commands.NugetManager.Model.Packages
{
    public class PackageInfo : IPackageInfo
    {
        public PackageInfo()
        {
            // no member
        }

        public PackageInfo(string packageId, IImmutableSet<string> versions)
        {
            Id = packageId;
            Versions = versions;
        }

        public string Id { get; set; }

        public IImmutableSet<string> Versions { get; set; }
    }
}
