using System.Collections.Immutable;
using BeaverSoft.Texo.Commands.FileManager.Extensions;
using BeaverSoft.Texo.Commands.FileManager.Stage;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Markdown.Builder;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Commands.FileManager.Stash
{
    public class StashCommand : ModularCommand
    {
        private readonly IStashService stashes;
        private readonly IStageService stage;

        public StashCommand(IStashService stashes, IStageService stage)
        {
            this.stashes = stashes;
            this.stage = stage;

            RegisterQuery(StashQueries.LIST, List);
            RegisterQuery(StashQueries.SHOW, Show);
            RegisterQuery(StashQueries.PUSH, Push);
            RegisterQuery(StashQueries.APPLY, Apply);
            RegisterQuery(StashQueries.PEEK, Peek);
            RegisterQuery(StashQueries.POP, Pop);
            RegisterQuery(StashQueries.DROP, Drop);
            RegisterQuery(StashQueries.CLEAR, Clear);
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
                result.Add(BuildStashOverviewItem(stash, i));
            }

            return new ItemsResult(result.ToImmutable());
        }

        private ICommandResult Show(CommandContext context)
        {
            string id = context.GetParameterValue(StashParameters.IDENTIFIER);
            IStashEntry stash = int.TryParse(id, out int index)
                ? stashes.GetStash(index)
                : stashes.GetStash(id);

            if (stash == null)
            {
                return new ErrorTextResult($"No stash @{id} found.");
            }

            return new ItemsResult(BuildStashDetailItem(stash, index));
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
            return new ItemsResult(BuildStashDetailItem(stash, 0));
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

            ApplyStashToStage(stash, !context.HasOption(StashOptions.ADD));
            return new TextResult($"{GetStashHeader(stash, index)} has been applied to stage.");
        }

        private ICommandResult Peek(CommandContext context)
        {
            IStashEntry stash = stashes.GetStash(0);

            if (stash == null)
            {
                return new TextResult("The stash stack is empty.");
            }

            ApplyStashToStage(stash, !context.HasOption(StashOptions.ADD));
            return new ItemsResult($"{GetStashHeader(stash, 0)} has been applied to stage.");
        }

        private ICommandResult Pop(CommandContext context)
        {
            IStashEntry stash = stashes.GetStash(0);

            if (stash == null)
            {
                return new TextResult("The stash stack is empty.");
            }

            ApplyStashToStage(stash, !context.HasOption(StashOptions.ADD));
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

        private void ApplyStashToStage(IStashEntry stash, bool replace)
        {
            if (replace)
            {
                stage.Clear();
            }

            stage.SetLobby(stash.GetLobby());
            stage.Add(stash);
        }

        private static Item BuildStashDetailItem(IStashEntry stash, int index)
        {
            MarkdownBuilder builder = new MarkdownBuilder();
            builder.Header(GetStashHeader(stash, index));
            builder.Italic(GetStashLobbyTitle(stash));
            builder.WriteLine();
            builder.WritePathLists(stash.GetPaths(), stash.GetLobby());
            return Item.AsMarkdown(builder.ToString());
        }

        private static Item BuildStashOverviewItem(IStashEntry stash, int index)
        {
            MarkdownBuilder builder = new MarkdownBuilder();
            builder.Header(GetStashHeader(stash, index));
            builder.Bullet();
            builder.Italic(GetStashLobbyTitle(stash));
            builder.WritePathOverview(stash.GetPaths(), stash.GetLobby());
            return Item.AsMarkdown(builder.ToString());
        }

        private static string GetStashLobbyTitle(IStashEntry stash)
        {
            return string.IsNullOrEmpty(stash.GetLobby())
                ? "No lobby directory."
                : stash.GetLobby();
        }

        private static string GetStashHeader(IStashEntry stash, int index)
        {
            return string.IsNullOrWhiteSpace(stash.Name)
                ? $"Stash @{index}"
                : $"Stash @{stash.Name}";
        }
    }
}
