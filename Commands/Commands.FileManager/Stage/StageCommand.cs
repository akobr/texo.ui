using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BeaverSoft.Texo.Commands.FileManager.Extensions;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Extensions;
using BeaverSoft.Texo.Core.Markdown.Builder;
using BeaverSoft.Texo.Core.Path;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Commands.FileManager.Stage
{
    public class StageCommand : InlineIntersectionCommand
    {
        private readonly IStageService stage;

        public StageCommand(IStageService stage)
        {
            this.stage = stage ?? throw new ArgumentNullException(nameof(stage));

            RegisterQueryMethod(StageQueries.STATUS, Status);
            RegisterQueryMethod(StageQueries.LIST, List);
            RegisterQueryMethod(StageQueries.LOBBY, Lobby);
            RegisterQueryMethod(StageQueries.ADD, Add);
            RegisterQueryMethod(StageQueries.REMOVE, Remove);
        }

        private ICommandResult Status(CommandContext context)
        {
            return BuildStageResult(stage.GetLobby(), stage.GetPaths());
        }

        private ICommandResult List(CommandContext context)
        {
            if (context.HasPath())
            {
                string current = Environment.CurrentDirectory;
                return BuildStageResult(current, Directory.GetFileSystemEntries(current), current);
            }

            var pathValues = context.Parameters[ParameterKeys.PATH].GetValues();
            string header = pathValues.Count < 2
                ? pathValues[0]
                : $"{pathValues.Count} paths";

            return BuildStageResult(
                string.Empty,
                context.GetParameterPaths().SelectMany(path => path.GetItems()),
                header);

        }

        private ICommandResult Lobby(CommandContext context)
        {
            if (context.HasOption(StageOptions.DELETE))
            {
                stage.RemoveLobby();
                return new TextResult("Lobby has been removed from the stage.");
            }

            string lobbyPath = context.GetParameterValue(FileManagerParameters.SIMPLE_PATH);
            stage.SetLobby(lobbyPath);
            return new TextResult($"Lobby of the stage: {stage.GetLobby()}");
        }

        private ICommandResult Add(CommandContext context)
        {
            var beforePaths = stage.GetPaths();

            foreach (TexoPath path in context.GetParameterPaths())
            {
                stage.Add(path.GetItems());
            }

            var afterPaths = stage.GetPaths();
            var newPaths = afterPaths.RemoveAll(beforePaths);

            if (newPaths.IsNullOrEmpty())
            {
                return new TextResult("No new path has been added to the stage.");
            }

            return BuildStageResult(stage.GetLobby(), newPaths, "Added to the stage");
        }

        private ICommandResult Remove(CommandContext context)
        {
            var beforePaths = stage.GetPaths();

            foreach (TexoPath path in context.GetParameterPaths())
            {
                stage.Remove(path.GetItems());
            }

            var afterPaths = stage.GetPaths();
            var removedPaths = beforePaths.RemoveAll(afterPaths);

            if (removedPaths.IsNullOrEmpty())
            {
                return new TextResult("No path has been removed from the stage.");
            }

            return BuildStageResult(stage.GetLobby(), removedPaths, "Removed from the stage");
        }

        private static ICommandResult BuildStageResult(string lobbyPath, IEnumerable<string> paths, string header = "Stage")
        {
            MarkdownBuilder builder = new MarkdownBuilder();
            builder.Header(header);

            if (string.IsNullOrEmpty(lobbyPath))
            {
                builder.Italic("No lobby directory.");
            }
            else
            {
                builder.CodeInline(lobbyPath);
            }

            builder.WriteLine();
            builder.WritePathLists(lobbyPath, paths);

            return new ItemsResult(Item.Markdown(builder.ToString()));
        }
    }
}
