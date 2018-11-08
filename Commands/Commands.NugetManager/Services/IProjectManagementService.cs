using System.Collections.Immutable;
using BeaverSoft.Texo.Commands.NugetManager.Model.Projects;

namespace BeaverSoft.Texo.Commands.NugetManager.Services
{
    public interface IProjectManagementService
    {
        IProject Get(string projectPath);

        IImmutableList<IProject> FindProjects(string searchTerm);

        IImmutableList<IProject> FindProjectsByPackage(string packageId);
    }
}
