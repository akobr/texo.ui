using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using BeaverSoft.Texo.Commands.NugetManager.Model;
using BeaverSoft.Texo.Commands.NugetManager.Processing;

namespace BeaverSoft.Texo.Commands.NugetManager.Services
{
    public class PackageManagementService : IPackageManagementService
    {
        private readonly ISourceManagementService sources;
        private ImmutableSortedDictionary<string, IPackageInfo> packages;

        public PackageManagementService(ISourceManagementService sources)
        {
            this.sources = sources ?? throw new ArgumentNullException(nameof(sources));
            ResetPackages();
        }

        public IEnumerable<IPackageInfo> GetAllPackages()
        {
            return packages.Values;
        }

        IPackageInfo IPackageSource.GetPackage(string packageId)
        {
            return GetOrFetch(packageId);
        }

        public IPackageInfo GetOrFetch(string packageId)
        {
            if (packages.TryGetValue(packageId, out IPackageInfo package))
            {
                return package;
            }

            package = sources.FetchPackage(packageId);

            if (package == null)
            {
                return null;
            }

            packages = packages.SetItem(package.Id, package);
            return package;
        }

        public IEnumerable<IPackageInfo> SearchPackages(string searchTerm)
        {
            return sources.SearchPackages(searchTerm).Values;
        }

        public void Clear()
        {
            ResetPackages();
        }

        private void ResetPackages()
        {
            packages = ImmutableSortedDictionary.Create<string, IPackageInfo>(new InsensitiveStringComparer());
        }
    }
}
