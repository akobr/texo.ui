using System;
using System.Collections.Immutable;
using System.Linq;
using BeaverSoft.Texo.Commands.NugetManager.Model;
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

            foreach (string packageTerm in context.GetParameterValues(NugetManagerParameters.SEARCH_TERM))
            {
                foreach (IPackageInfo package in packages.Search(packageTerm))
                {
                    if (string.Equals(package.Id, packageTerm, StringComparison.OrdinalIgnoreCase))
                    {
                        items.Add(BuildPackageResult(packages.Fetch(package.Id)));
                    }
                    else
                    {
                        items.Add(BuildPackageResult(package));
                    }
                }
            }

            if (items.Count < 1)
            {
                return new TextResult("No package found.");
            }

            return new ItemsResult(items.ToImmutable());
        }

        private Item BuildPackageResult(IPackageInfo package)
        {
            MarkdownBuilder builder = new MarkdownBuilder();
            builder.Header(package.Id);
            builder.Italic("Newest version:");
            builder.Write(" ");
            builder.Write(package.Versions.Min);
            AddVersionsToResult(package, builder);
            AddProjectsToResult(package, builder);
            return Item.AsMarkdown(builder.ToString());
        }

        private void AddVersionsToResult(IPackageInfo package, MarkdownBuilder builder)
        {
            if (package.Versions.Count < 2)
            {
                return;
            }

            builder.Header($"Available in {package.Versions.Count} version(s)", 2);

            foreach (string version in package.Versions.Take(50))
            {
                builder.Bullet(version);
            }
        }

        private void AddProjectsToResult(IPackageInfo package, MarkdownBuilder builder)
        {
            IImmutableList<IProject> usedInProjects = projects.FindByPackage(package.Id);

            if (usedInProjects.IsNullOrEmpty())
            {
                return;
            }

            builder.Header($"Used in {usedInProjects.Count} project(s)", 2);

            foreach (IProject project in usedInProjects)
            {
                builder.Bullet($"{project.Name} ({project.Packages[package.Id].Version})");
            }
        }
    }
}
