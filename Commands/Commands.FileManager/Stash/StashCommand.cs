using System.Collections.Immutable;
using BeaverSoft.Texo.Commands.FileManager.Extensions;
using BeaverSoft.Texo.Commands.FileManager.Stage;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Markdown.Builder;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Commands.FileManager.Stash
{
    public class StashCommand : InlineIntersectionCommand
    {
        private readonly IStashService stashes;
        private readonly IStageService stage;

        public StashCommand(IStashService stashes, IStageService stage)
        {
            this.stashes = stashes;
            this.stage = stage;

            RegisterQueryMethod(StashQueries.LIST, List);
            RegisterQueryMethod(StashQueries.PUSH, Push);
            RegisterQueryMethod(StashQueries.APPLY, Apply);
            RegisterQueryMethod(StashQueries.PEEK, Peek);
            RegisterQueryMethod(StashQueries.POP, Pop);
            RegisterQueryMethod(StashQueries.DROP, Drop);
            RegisterQueryMethod(StashQueries.CLEAR, Clear);
        }

        private ICommandResult List(CommandContext context)
        {
            var allStashes = stashes.GetStashes();

            if (allStashes.Count < 1)
            {
                return new TextResult("The stash stack is empty.");
            }

            var result = ImmutableList<Item>.Empty.ToBuilder();

            for (int i = 0; i < allStashes.Count; i++)
            {
                IStashEntry stash = allStashes[i];
                result.Add(BuildStashItem(stash, i));
            }

            return new ItemsResult(result.ToImmutable());
        }

        private ICommandResult Push(CommandContext context)
        {
            IImmutableList<string> paths = stage.GetPaths();

            if (paths == null || paths.Count < 1)
            {
                return new ErrorTextResult("The stage is empty.");
            }

            string name = context.GetParameterValue(ParameterKeys.NAME);
            IStashEntry stash = stashes.CreateStash(stage.GetLobby(), paths, name);
            return new ItemsResult(BuildStashItem(stash, 0));
        }

        private ICommandResult Apply(CommandContext context)
        {
            if (!context.HasParameter(StashParameters.IDENTIFIER))
            {
                return Peek(context);
            }

            string id = context.GetParameterValue(StashParameters.IDENTIFIER);
            IStashEntry stash = int.TryParse(id, out int index)
                ? stashes.GetStash(index)
                : stashes.GetStash(id);

            if (stash == null)
            {
                return new ErrorTextResult($"No stash @{id} found.");
            }

            ApplyStashToStage(stash);
            return new TextResult($"{GetStashHeader(stash, index)} has been applied to stage.");
        }

        private ICommandResult Peek(CommandContext context)
        {
            IStashEntry stash = stashes.GetStash(0);

            if (stash == null)
            {
                return new TextResult("The stash stack is empty.");
            }

            ApplyStashToStage(stash);
            return new ItemsResult($"{GetStashHeader(stash, 0)} has been applied to stage.");
        }

        private ICommandResult Pop(CommandContext context)
        {
            IStashEntry stash = stashes.GetStash(0);

            if (stash == null)
            {
                return new TextResult("The stash stack is empty.");
            }

            ApplyStashToStage(stash);
            stashes.RemoveStash(stash);
            return new TextResult($"{GetStashHeader(stash, 0)} has been applied to stage and removed from top of the stack.");
        }

        private ICommandResult Drop(CommandContext context)
        {
            string id = context.GetParameterValue(StashParameters.IDENTIFIER);
            IStashEntry stash = int.TryParse(id, out int index)
                ? stashes.GetStash(index)
                : stashes.GetStash(id);

            if (stash == null)
            {
                return new ErrorTextResult($"No stash @{id} found.");
            }

            stashes.RemoveStash(stash);
            return new TextResult($"{GetStashHeader(stash, index)} has been dropped.");
        }

        private ICommandResult Clear(CommandContext context)
        {
            stashes.Clean();
            return new TextResult("All stashes have been removed.");
        }

        private void ApplyStashToStage(IStashEntry stash)
        {
            stage.Clear();
            stage.SetLobby(stash.LobbyPath);
            stage.Add(stash);
        }

        private static Item BuildStashItem(IStashEntry stash, int index)
        {
            MarkdownBuilder builder = new MarkdownBuilder();
            builder.Header(GetStashHeader(stash, index));
            builder.Italic(GetStashLobbyTitle(stash));
            builder.WriteLine();
            builder.WritePathLists(stash.Paths, stash.LobbyPath);
            return Item.Markdown(builder.ToString());
        }

        private static string GetStashLobbyTitle(IStashEntry stash)
        {
            return string.IsNullOrEmpty(stash.LobbyPath)
                ? "No lobby directory."
                : stash.LobbyPath;
        }

        private static string GetStashHeader(IStashEntry stash, int index)
        {
            return string.IsNullOrWhiteSpace(stash.Name)
                ? $"Stash @{index}"
                : $"Stash @{stash.Name}";
        }
    }
}
