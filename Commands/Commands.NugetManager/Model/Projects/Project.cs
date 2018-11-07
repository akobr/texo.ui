using System;
using System.Collections.Immutable;
using BeaverSoft.Texo.Commands.NugetManager.Extenssions;
using BeaverSoft.Texo.Commands.NugetManager.Model.Packages;

namespace BeaverSoft.Texo.Commands.NugetManager.Model.Projects
{
    public class Project : IProject
    {
        public Project(Uri path, IImmutableList<IPackage> packages)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Packages = packages ?? throw new ArgumentNullException(nameof(packages));
            Id = path.AbsolutePath.GetPathId();
        }

        public string Id { get; }

        public Uri Path { get; }

        public IImmutableList<IPackage> Packages { get; }
    }
}
