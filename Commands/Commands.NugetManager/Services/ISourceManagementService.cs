using System.Collections.Immutable;
using BeaverSoft.Texo.Commands.NugetManager.Packages;

namespace BeaverSoft.Texo.Commands.NugetManager.Services
{
    public interface ISourceManagementService
    {
        IImmutableDictionary<string, IPackageInfo> GetPackages(string searchTerm);
    }
}
