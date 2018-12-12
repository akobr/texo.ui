using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
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
            RegisterQueryMethod(StageQueries.ADD, Add);
            RegisterQueryMethod(StageQueries.REMOVE, Remove);
            RegisterQueryMethod(StageQueries.LOBBY, Lobby);
            RegisterQueryMethod(StageQueries.REMOVE_LOBBY, RemoveLobby);
        }

        private ICommandResult Status(CommandContext context)
        {
            var paths = stage.GetPaths();

            if (paths.Count < 1)
            {
                return new TextResult("The stage is empty.");
            }

            return new ItemsResult(BuildStageItem(stage.GetLobby(), stage.GetPaths(), "The stage"));
        }

        private static ICommandResult List(CommandContext context)
        {
            if (!context.HasParameter(ParameterKeys.PATH))
            {
                string current = Environment.CurrentDirectory;
                return new ItemsResult(BuildStageItem(current, Directory.GetFileSystemEntries(current), current));
            }

            var items = ImmutableList<Item>.Empty.ToBuilder();

            foreach (TexoPath path in context.GetParameterPaths())
            {
                items.Add(BuildStageItem(path.GetFixedPrefixPath(), path.GetItems(), path.Path));
            }

            return new ItemsResult(items.ToImmutable());
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

            return new ItemsResult(BuildStageItem(stage.GetLobby(), newPaths, "Added to the stage"));
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

            return new ItemsResult(BuildStageItem(stage.GetLobby(), removedPaths, "Removed from the stage"));
        }

        private ICommandResult Lobby(CommandContext context)
        {
            string lobbyPath = context.GetParameterValue(FileManagerParameters.SIMPLE_PATH);
            string actualLobby = stage.GetLobby();

            if (actualLobby.IsSamePath(lobbyPath))
            {
                return new TextResult($"The stage already has lobby: {actualLobby}");
            }

            stage.SetLobby(lobbyPath);
            return new TextResult($"Lobby of the stage: {stage.GetLobby()}");
        }

        private ICommandResult RemoveLobby(CommandContext arg)
        {
            stage.RemoveLobby();
            return new TextResult("Lobby has been removed from the stage.");
        }

        private static Item BuildStageItem(string lobbyPath, IEnumerable<string> paths, string header)
        {
            MarkdownBuilder builder = new MarkdownBuilder();
            builder.Header(header);

            builder.Italic(string.IsNullOrEmpty(lobbyPath)
                ? "No lobby directory."
                : lobbyPath);

            builder.WriteLine();
            builder.WritePathLists(lobbyPath, paths);

            return Item.Markdown(builder.ToString());
        }
    }
}
