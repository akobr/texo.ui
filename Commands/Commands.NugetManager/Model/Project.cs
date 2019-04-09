using System;
using System.Collections.Immutable;

namespace BeaverSoft.Texo.Commands.NugetManager.Model
{
    public class Project : IProject
    {
        public Project()
        {
            // no operation
        }

        public Project(string path, IImmutableDictionary<string, IPackage> packages)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Packages = packages ?? throw new ArgumentNullException(nameof(packages));
            Name = System.IO.Path.GetFileNameWithoutExtension(path);
        }

        public string Name { get; set; }

        public string Path { get; set; }

        public IImmutableDictionary<string, IPackage> Packages { get; set; }
    }
}
