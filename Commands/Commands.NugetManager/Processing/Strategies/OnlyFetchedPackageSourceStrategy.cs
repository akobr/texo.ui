using BeaverSoft.Texo.Commands.NugetManager.Model;
using BeaverSoft.Texo.Commands.NugetManager.Services;

namespace BeaverSoft.Texo.Commands.NugetManager.Processing.Strategies
{
    public class OnlyFetchedPackageSourceStrategy : IPackageSource
    {
        private readonly IPackageManagementService packages;

        public OnlyFetchedPackageSourceStrategy(IPackageManagementService packages)
        {
            this.packages = packages;
        }

        public IPackageInfo GetPackage(string packageId)
        {
            return packages.GetPackage(packageId);
        }
    }
}
