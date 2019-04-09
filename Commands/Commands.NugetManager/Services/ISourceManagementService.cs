using System.Collections.Generic;
using System.Collections.Immutable;
using BeaverSoft.Texo.Commands.NugetManager.Model;

namespace BeaverSoft.Texo.Commands.NugetManager.Services
{
    public interface ISourceManagementService
    {
        IEnumerable<string> GetAllSources();

        void Add(string source);

        void AddRange(IEnumerable<string> newSources);

        IPackageInfo FetchPackage(string packageId);

        IImmutableDictionary<string, IPackageInfo> SearchPackages(string searchTerm);

        void Clear();
    }
}
