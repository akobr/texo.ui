using System;
namespace BeaverSoft.Texo.Commands.NugetManager.Model.Packages
{
    public class Package : IPackage
    {
        public Package()
        {
            // no operation
        }

        public Package(string id, string version, bool canBeUpdated)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Version = version ?? throw new ArgumentNullException(nameof(version));
            CanBeUpdated = canBeUpdated;
        }

        public string Id { get; set; }

        public string Version { get; set; }

        public bool CanBeUpdated { get; set; }
    }
}
