using System.Collections.Generic;
using System.Collections.Immutable;
using BeaverSoft.Texo.Commands.NugetManager.Model.Projects;

namespace BeaverSoft.Texo.Commands.NugetManager.Services
{
    public interface IProjectManagementService
    {
        IEnumerable<IProject> GetAllProjects();

        IProject Get(string projectPath);

        void AddOrUpdate(IProject project);

        void Remove(IProject project);

        IImmutableList<IProject> FindProjects(string searchTerm);

        IImmutableList<IProject> FindProjectsByPackage(string packageId);
    }
}
