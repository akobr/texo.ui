using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using BeaverSoft.Texo.Commands.NugetManager.Model.Packages;

namespace BeaverSoft.Texo.Commands.NugetManager.Services
{
    public class PackageManagementService : IPackageManagementService
    {
        private ImmutableSortedDictionary<string, IPackageInfo> packages;

        public PackageManagementService()
        {
            ResetPackages();
        }

        public IEnumerable<IPackageInfo> GetAllPackages()
        {
            return packages.Values;
        }

        public IPackageInfo Get(string packageId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPackageInfo> FindPackages(string searchTerm)
        {
            throw new NotImplementedException();
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
