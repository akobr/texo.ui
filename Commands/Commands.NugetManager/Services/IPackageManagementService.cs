using System.Collections.Generic;
using BeaverSoft.Texo.Commands.NugetManager.Model;
using BeaverSoft.Texo.Commands.NugetManager.Processing;

namespace BeaverSoft.Texo.Commands.NugetManager.Services
{
    public interface IPackageManagementService : IPackageSource
    {
        IEnumerable<IPackageInfo> GetAll();

        bool TryGet(string packageId, out IPackageInfo package);

        IPackageInfo GetOrFetch(string packageId);

        IPackageInfo Fetch(string packageId);

        IEnumerable<IPackageInfo> Search(string searchTerm);

        void Clear();
    }
}
