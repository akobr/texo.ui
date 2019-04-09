using BeaverSoft.Texo.Commands.NugetManager.Model;

namespace BeaverSoft.Texo.Commands.NugetManager.Processing
{
    public interface IPackageSource
    {
        IPackageInfo GetPackage(string packageId);
    }
}
