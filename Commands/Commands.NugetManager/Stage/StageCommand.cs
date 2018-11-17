using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BeaverSoft.Texo.Commands.FileManager.Stage;
using BeaverSoft.Texo.Commands.NugetManager.Model;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Extensions;
using BeaverSoft.Texo.Core.Markdown.Builder;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Commands.NugetManager.Stage
{
    public class StageCommand : InlineIntersectionCommand
    {
        private readonly IStageService stage;

        public StageCommand(IStageService stage)
        {
            this.stage = stage ?? throw new ArgumentNullException(nameof(stage));

            RegisterQueryMethod(StageQueries.STATUS, Status);
            RegisterQueryMethod(StageQueries.ADD, Add);
            RegisterQueryMethod(StageQueries.REMOVE, Remove);
        }

        private ICommandResult Status(CommandContext context)
        {
            var projects = stage.GetProjects();

            if (projects.Count < 1)
            {
                return new TextResult("The stage is empty.");
            }

            var items = ImmutableList<Item>.Empty.ToBuilder();

            if (context.HasOption(StageOptions.CONFIGS))
            {
                items.AddRange(BuildConfigs());
            }

            if (context.HasOption(StageOptions.SOURCES))
            {
                items.Add(BuildSourcesItem());
            }

            if (context.Options.Count < 1 || context.HasOption(StageOptions.PROJECTS))
            {
                items.AddRange(BuildProjectItems(projects));
            }

            return new ItemsResult(items.ToImmutable());
        }

        private ICommandResult Add(CommandContext context)
        {
            var beforeProjects = stage.GetProjects();
            stage.Add(context.GetParameterValues(ParameterKeys.PATH));
            var afterProjects = stage.GetProjects();
            var newProjects = afterProjects.RemoveAll(beforeProjects);

            if (newProjects.Count < 1)
            {
                return new TextResult("No project has been added.");
            }

            return new ItemsResult(BuildProjectItems(newProjects).ToImmutableList());
        }

        private ICommandResult Remove(CommandContext context)
        {
            var beforeProjects = stage.GetProjects();
            stage.Remove(context.GetParameterValues(ParameterKeys.PATH));
            var afterProjects = stage.GetProjects();
            var removedProjects = beforeProjects.RemoveAll(afterProjects);

            if (removedProjects.Count < 1)
            {
                return new TextResult("No project has been removed.");
            }

            return new ItemsResult(removedProjects.Select(p => Item.Plain(p.Name)).ToImmutableList());
        }

        private IEnumerable<Item> BuildConfigs()
        {
            List<Item> result = new List<Item>();

            foreach (IConfig config in stage.GetConfigs())
            {
                MarkdownBuilder builder = new MarkdownBuilder();
                builder.Header(config.Path);

                foreach (string source in config.Sources)
                {
                    builder.Bullet(source);
                }

                result.Add(Item.Markdown(builder.ToString()));
            }

            return result;
        }

        private Item BuildSourcesItem()
        {
            MarkdownBuilder builder = new MarkdownBuilder();
            builder.Header("Source(s)");

            foreach (string source in stage.GetSources())
            {
                builder.Bullet(source);
            }

            return Item.Markdown(builder.ToString());
        }

        private static IEnumerable<Item> BuildProjectItems(IEnumerable<IProject> projects)
        {
            List<Item> result = new List<Item>();

            foreach (IProject project in projects)
            {
                MarkdownBuilder builder = new MarkdownBuilder();
                builder.Header(project.Name);
                builder.WriteLine(project.Path);
                builder.Italic($"{project.Packages.Count} package(s) referenced.");
                builder.WriteLine();

                foreach (IPackage package in project.Packages.Values)
                {
                    BuildProjectItem(builder, package);
                }

                result.Add(Item.Markdown(builder.ToString()));
            }

            return result;
        }

        private static void BuildProjectItem(MarkdownBuilder builder, IPackage package)
        {
            builder.Bullet();

            if (package.CanBeUpdated)
            {
                builder.Bold(package.Id);
            }
            else
            {
                builder.Write(package.Id);
            }

            builder.WriteLine($" ({package.Version})");
        }
    }
}