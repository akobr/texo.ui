using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using BeaverSoft.Texo.Commands.NugetManager.Model.Packages;
using NuGet;

namespace BeaverSoft.Texo.Commands.NugetManager.Services
{
    public class SourceManagementService : ISourceManagementService
    {
        private const string OFFICIAL_PACKAGE_REPOSITORY_URL = "https://packages.nuget.org/api/v2";

        private ImmutableHashSet<string> sources;

        public IEnumerable<string> GetAllSources()
        {
            return sources;
        }

        public IPackageInfo GetPackage(string packageId)
        {
            IPackageInfo result = null;

            foreach (string source in sources)
            {
                IPackageInfo package = GetPackageFromRepository(packageId, source);
                result = result == null
                    ? package
                    : new PackageInfo(result.Id, result.Versions.Union(package.Versions));
            }

            return result;
        }

        public IImmutableDictionary<string, IPackageInfo> GetPackages(string searchTerm)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            sources = ImmutableHashSet<string>.Empty;
            sources.Add(OFFICIAL_PACKAGE_REPOSITORY_URL);
        }

        private IEnumerable<IPackageInfo> GetPackagesFromRepository(string searchTerm, string repositoryUrl)
        {
            IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository(repositoryUrl);
            var versions = new Dictionary<string, ImmutableSortedSet<string>.Builder>();
            var previousPackageVersions = ImmutableSortedSet.CreateBuilder<string>(new InsensitiveOpositeStringComparer()); ;
            string previousPackageId = string.Empty;

            foreach (NuGet.IPackage nugetPackage in repo.Search(searchTerm, true))
            {
                if (!string.Equals(previousPackageId, nugetPackage.Id, StringComparison.OrdinalIgnoreCase))
                {
                    previousPackageId = nugetPackage.Id;

                    if (!versions.TryGetValue(previousPackageId, out previousPackageVersions))
                    {
                        previousPackageVersions = ImmutableSortedSet.CreateBuilder<string>(new InsensitiveOpositeStringComparer());
                        versions[previousPackageId] = previousPackageVersions;
                    }
                }

                previousPackageVersions.Add(nugetPackage.Version.ToNormalizedString());
            }

            foreach (var packagePair in versions)
            {
                yield return new PackageInfo(packagePair.Key, packagePair.Value.ToImmutable());
            }
        }

        private static IPackageInfo GetPackageFromRepository(string packageId, string repositoryUrl)
        {
            IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository(repositoryUrl);
            var versions = ImmutableSortedSet.CreateBuilder<string>(new InsensitiveOpositeStringComparer());
            NuGet.IPackage lastNugetPackage = null;

            foreach (NuGet.IPackage nugetPackage in repo.FindPackagesById(packageId))
            {
                lastNugetPackage = nugetPackage;
                versions.Add(nugetPackage.Version.ToNormalizedString());
            }

            if (lastNugetPackage == null)
            {
                return null;
            }

            return new PackageInfo(lastNugetPackage.Id, versions.ToImmutable());
        }
    }
}
