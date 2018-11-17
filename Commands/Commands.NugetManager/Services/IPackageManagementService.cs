using System.Collections.Generic;
using BeaverSoft.Texo.Commands.NugetManager.Model;
using BeaverSoft.Texo.Commands.NugetManager.Processing;

namespace BeaverSoft.Texo.Commands.NugetManager.Services
{
    public interface IPackageManagementService : IPackageSource
    {
        IEnumerable<IPackageInfo> GetAllPackages();

        IPackageInfo GetOrFetch(string packageId);

        IEnumerable<IPackageInfo> SearchPackages(string searchTerm);

        void Clear();
    }
}
