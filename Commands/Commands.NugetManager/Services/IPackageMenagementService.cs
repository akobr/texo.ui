using BeaverSoft.Texo.Commands.NugetManager.Model.Packages;

namespace BeaverSoft.Texo.Commands.NugetManager.Services
{
    public interface IPackageMenagementService
    {
        IPackageInfo Get(string packageId);
    }
}
