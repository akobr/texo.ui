using System;
using Newtonsoft.Json;

namespace BeaverSoft.Texo.Commands.NugetManager.Model.Packages
{
    public class Package : IPackage
    {
        public Package()
        {
            // no operation
        }

        public Package(string id, string version)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Version = version ?? throw new ArgumentNullException(nameof(version));
        }

        public string Id { get; set; }

        public string Version { get; set; }
    }
}
