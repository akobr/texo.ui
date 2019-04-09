namespace BeaverSoft.Texo.Commands.NugetManager.Services
{
    public class ManagementService : IManagementService
    {
        public ManagementService(
            IProjectManagementService projects,
            IPackageManagementService packages,
            IConfigManagementService configs,
            ISourceManagementService sources)
        {
            Projects = projects;
            Packages = packages;
            Configs = configs;
            Sources = sources;
        }

        public IProjectManagementService Projects { get; }

        public IPackageManagementService Packages { get; }

        public IConfigManagementService Configs { get; }

        public ISourceManagementService Sources { get; }
    }
}
