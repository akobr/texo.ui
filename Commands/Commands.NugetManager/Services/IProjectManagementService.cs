using BeaverSoft.Texo.Commands.NugetManager.Model.Projects;

namespace BeaverSoft.Texo.Commands.NugetManager.Services
{
    public interface IProjectManagementService
    {
        IProject Get(string projectPath);
    }
}
