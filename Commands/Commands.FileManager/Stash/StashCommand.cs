using System.Collections.Generic;
using System.Collections.Immutable;
using BeaverSoft.Texo.Commands.FileManager.Stage;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Markdown.Builder;
using BeaverSoft.Texo.Core.Path;
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
                return new TextResult("Stash stack is empty.");
            }

            var result = ImmutableList<Item>.Empty.ToBuilder();

            for (int i = 0; i < allStashes.Count; i++)
            {
                IStashEntry stash = allStashes[i];
                result.Add(BuildStashItem(stash, i));
            }

            return new ItemsResult(result.ToImmutable());
        }

        private ICommandResult Apply(CommandContext context)
        {
            string id = context.GetParameterValue(StashParameters.IDENTIFIER);
            IStashEntry stash = int.TryParse(id, out int index)
                ? stashes.GetStash(index)
                : stashes.GetStash(id);

            if (stash == null)
            {
                return new ErrorTextResult($"No stash with identifier: {id}");
            }

            stage.Clear();
            stage.Add(stash);

            return new ItemsResult(BuildStashItem(stash, index));
        }

        private ICommandResult Peek(CommandContext context)
        {
            IStashEntry stash = stashes.GetStash(0);

            if (stash == null)
            {
                return new TextResult("Stash stack is empty.");
            }

            stage.Clear();
            stage.Add(stash);

            return new ItemsResult(BuildStashItem(stash, 0));
        }

        private ICommandResult Pop(CommandContext context)
        {
            IImmutableList<string> paths = stage.GetPaths();

            if (paths == null || paths.Count < 1)
            {
                return new ErrorTextResult("Stage is empty.");
            }

            string name = context.GetParameterValue(ParameterKeys.NAME);
            IStashEntry stash = stashes.CreateStash(paths, name);
            return new ItemsResult(BuildStashItem(stash, 0));
        }

        private ICommandResult Drop(CommandContext context)
        {
            string id = context.GetParameterValue(StashParameters.IDENTIFIER);
            IStashEntry stash = int.TryParse(id, out int index)
                ? stashes.GetStash(index)
                : stashes.GetStash(id);

            if (stash == null)
            {
                return new ErrorTextResult($"No stash with identifier: {id}");
            }

            stashes.RemoveStash(stash);
            return new TextResult($"The stash has been dropped: {id}");
        }

        private ICommandResult Clear(CommandContext context)
        {
            stashes.Clean();
            return new TextResult("All stashes have been dropped.");
        }

        private Item BuildStashItem(IStashEntry stash, int index)
        {
            MarkdownBuilder builder = new MarkdownBuilder();
            builder.Header(string.IsNullOrWhiteSpace(stash.Name) ? $"Stash @{index}" : $"Stash @{stash.Name}");

            List<string> directories = new List<string>();
            List<string> files = new List<string>();
            List<string> nonExists = new List<string>();

            foreach (string path in stash.Paths)
            {
                PathTypeEnum type = path.GetPathType();

                switch (type)
                {
                    case PathTypeEnum.Directory:
                        directories.Add(path.GetRelativePath());
                        break;

                    case PathTypeEnum.File:
                        files.Add(path.GetRelativePath());
                        break;

                    case PathTypeEnum.NonExistent:
                        nonExists.Add(path.GetRelativePath());
                        break;

                }
            }

            WritePaths(directories, "Directories", builder);
            WritePaths(files, "Files", builder);
            WritePaths(nonExists, "Non-Existing", builder);

            return Item.Markdown(builder.ToString());
        }

        private static void WritePaths(List<string> paths, string title, MarkdownBuilder builder)
        {
            if (paths.Count <= 0)
            {
                return;
            }

            paths.Sort();
            builder.Header(title, 2);
            builder.WriteLine();

            foreach (string directory in paths)
            {
                builder.Bullet(directory);
            }
        }
    }
}
