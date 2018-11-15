namespace BeaverSoft.Texo.Commands.NugetManager.Services
{
    public interface IManagementService
    {
        IProjectManagementService Projects { get; }

        IPackageManagementService Packages { get; }

        IConfigManagementService Configs { get; }

        ISourceManagementService Sources { get; }
    }
}
