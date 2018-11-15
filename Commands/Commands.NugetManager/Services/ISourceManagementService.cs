using System.Collections.Generic;
using System.Collections.Immutable;
using BeaverSoft.Texo.Commands.NugetManager.Model.Packages;
using BeaverSoft.Texo.Commands.NugetManager.Model.Sources;

namespace BeaverSoft.Texo.Commands.NugetManager.Services
{
    public interface ISourceManagementService
    {
        IEnumerable<ISource> GetAllSources();

        IImmutableDictionary<string, IPackageInfo> GetPackages(string searchTerm);

        void Clear();
    }
}
