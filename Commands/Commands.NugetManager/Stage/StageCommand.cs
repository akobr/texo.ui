using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BeaverSoft.Texo.Commands.FileManager.Stage;
using BeaverSoft.Texo.Commands.NugetManager.Model.Configs;
using BeaverSoft.Texo.Commands.NugetManager.Model.Packages;
using BeaverSoft.Texo.Commands.NugetManager.Model.Projects;
using BeaverSoft.Texo.Commands.NugetManager.Model.Sources;
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
            var items = ImmutableList<Item>.Empty.ToBuilder();

            if (context.HasOption(StageOptions.CONFIGS))
            {
                items.AddRange(BuildConfigs());
            }

            if (context.HasOption(StageOptions.SOURCES))
            {
                items.AddRange(BuildSources());
            }

            if (context.Options.Count < 1 || context.HasOption(StageOptions.PROJECTS))
            {
                items.AddRange(BuildProjectItems(stage.GetProjects()));
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
                builder.Header(config.Path, 2);

                foreach (ISourceInfo source in config.Sources)
                {
                    builder.Bullet(source.Address.AbsoluteUri);
                }

                result.Add(Item.Markdown(builder.ToString()));
            }

            return result;
        }

        private IEnumerable<Item> BuildSources()
        {
            List<Item> result = new List<Item>();

            foreach (ISource source in stage.GetSources())
            {
                result.Add(Item.Plain(source.Address.AbsoluteUri));
            }

            return result;
        }

        private static IEnumerable<Item> BuildProjectItems(IEnumerable<IProject> projects)
        {
            List<Item> result = new List<Item>();

            foreach (IProject project in projects)
            {
                MarkdownBuilder builder = new MarkdownBuilder();
                builder.Header(project.Name);
                builder.CodeInline(project.Path);
                builder.WriteLine();
                builder.Italic($"{project.Packages.Count} package(s) referenced.");
                builder.WriteLine();

                foreach (IPackage package in project.Packages.Values)
                {
                    builder.Bullet($"{package.Id} ({package.Version})");
                }

                result.Add(Item.Markdown(builder.ToString()));
            }

            return result;
        }
    }
}
