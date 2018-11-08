using System;

namespace BeaverSoft.Texo.Commands.NugetManager.Model.Packages
{
    public class Package : IPackage
    {
        public Package(string id, string version)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            FullVersion = version ?? throw new ArgumentNullException(nameof(version));

            if (!Version.TryParse(version, out Version newVersion))
            {
                int indexOfDash = version.IndexOf('-');

                if (indexOfDash > 0) 
                {
                    Version.TryParse(version.Substring(0, indexOfDash), out newVersion);
                }
            }

            Version = newVersion;
        }

        public string Id { get; }

        public string FullVersion { get; }

        public Version Version { get; }
    }
}
