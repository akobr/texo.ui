using System;
using System.Collections.Generic;
using System.Text;
using BeaverSoft.Texo.Commands.NugetManager.Services;
using BeaverSoft.Texo.Core.Commands;

namespace BeaverSoft.Texo.Commands.NugetManager.Manage
{
    public class ProjectCommand : ICommand
    {
        private readonly IProjectManagementService projects;
        private readonly IPackageManagementService packages;        

        public ProjectCommand(
            IProjectManagementService projects,
            IPackageManagementService packages)
        {
            this.projects = projects ?? throw new ArgumentNullException(nameof(projects));
            this.packages = packages ?? throw new ArgumentNullException(nameof(packages));
        }

        public ICommandResult Execute(CommandContext context)
        {
            throw new NotImplementedException();
        }
    }
}
