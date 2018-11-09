using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BeaverSoft.Texo.Commands.NugetManager.Model.Packages;
using BeaverSoft.Texo.Commands.NugetManager.Model.Projects;
using BeaverSoft.Texo.Commands.NugetManager.Services;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Extensions;
using BeaverSoft.Texo.Core.Markdown.Builder;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Commands.NugetManager.Manage
{
    public class PackageCommand : ICommand
    {
        private readonly IPackageManagementService packages;
        private readonly IProjectManagementService projects;

        public PackageCommand(
            IPackageManagementService packages,
            IProjectManagementService projects)
        {
            this.packages = packages ?? throw new ArgumentNullException(nameof(packages));
            this.projects = projects ?? throw new ArgumentNullException(nameof(projects));
        }

        public ICommandResult Execute(CommandContext context)
        {
            var items = ImmutableList<Item>.Empty.ToBuilder();
            bool allVersions = context.HasOption(ManageOptions.ALL_VERSIONS);

            foreach (string packageTerm in context.GetParameterValues(NugetManagerParameters.SEARCH_TERM))
            {
                foreach (IPackageInfo package in packages.FindPackages(packageTerm))
                {
                    items.Add(BuildPackageResult(package, allVersions));
                }
            }

            if (items.Count < 1)
            {
                return new TextResult("No package found.");
            }

            return new ItemsResult(items.ToImmutable());
        }

        private Item BuildPackageResult(IPackageInfo package, bool allVersions)
        {
            MarkdownBuilder builder = new MarkdownBuilder();
            builder.Header(package.Id);
            AddVersionsToResult(package, builder, allVersions);
            AddProjectsToResult(package, builder);
            return Item.Markdown(builder.ToString());
        }

        private void AddVersionsToResult(IPackageInfo package, MarkdownBuilder builder, bool allVersions)
        {
            builder.Header($"Available in {package.AllVersions.Count} version(s)", 2);

            IEnumerable<string> versionsToShow = allVersions
                ? package.AllVersions.Take(10)
                : package.AllVersions;

            foreach (string version in versionsToShow)
            {
                builder.Bullet(version);
            }
        }

        private void AddProjectsToResult(IPackageInfo package, MarkdownBuilder builder)
        {
            IImmutableList<IProject> usedInProjects = projects.FindProjectsByPackage(package.Id);

            if (usedInProjects.IsNullOrEmpty())
            {
                return;
            }

            builder.Header($"Used in {usedInProjects.Count} project(s)", 2);

            foreach (IProject project in usedInProjects)
            {
                builder.Bullet($"{project.Name} ({project.Packages[package.Id].Id})");
            }
        }
    }
}
