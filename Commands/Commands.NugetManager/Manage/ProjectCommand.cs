using System;
using System.Collections.Immutable;
using BeaverSoft.Texo.Commands.NugetManager.Services;
using BeaverSoft.Texo.Commands.NugetManager.Stage;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Commands.NugetManager.Manage
{
    public class ProjectCommand : ICommand
    {
        private readonly IProjectManagementService projects;        

        public ProjectCommand(IProjectManagementService projects)
        {
            this.projects = projects ?? throw new ArgumentNullException(nameof(projects));
        }

        public ICommandResult Execute(CommandContext context)
        {
            var items = ImmutableList<Item>.Empty.ToBuilder();

            foreach (string projectTerm in context.GetParameterValues(NugetManagerParameters.SEARCH_TERM))
            {
                StageCommand.BuildProjectItems(projects.Find(projectTerm));
            }

            if (items.Count < 1)
            {
                return new TextResult("No project found.");
            }

            return new ItemsResult(items.ToImmutable());
        }
    }
}
