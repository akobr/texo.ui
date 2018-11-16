using System.Collections.Generic;
using System.Collections.Immutable;
using BeaverSoft.Texo.Commands.NugetManager.Model.Packages;

namespace BeaverSoft.Texo.Commands.NugetManager.Services
{
    public interface ISourceManagementService
    {
        IEnumerable<string> GetAllSources();

        IPackageInfo GetPackage(string packageId);

        IImmutableDictionary<string, IPackageInfo> GetPackages(string searchTerm);

        void Clear();
    }
}
