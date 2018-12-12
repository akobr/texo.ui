using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using BeaverSoft.Texo.Commands.FileManager.Stage;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Path;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.View;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Commands.FileManager.Operations
{
    public class ContentSearchCommand : ICommand
    {
        private readonly IStageService stage;
        private readonly ILogService logger;

        public ContentSearchCommand(IStageService stage, ILogService logger)
        {
            this.stage = stage ?? throw new ArgumentNullException(nameof(stage));
            this.logger = logger;
        }

        public ICommandResult Execute(CommandContext context)
        {
            if (!stage.TryGetPaths(out var paths))
            {
                return new TextResult("The stage is empty.");
            }

            SearchContext copyContext = new SearchContext
            {
                Items = paths,
                SourceLobby = stage.GetLobby(),
                IsRegex = context.HasOption(ApplyOptions.CASE_SENSITIVE),
                IsCaseSensitive = context.HasOption(ApplyOptions.CASE_SENSITIVE),
                FileFilter = context.GetParameterFromOption(ApplyOptions.FILE_FILTER)
            };

            return Search(copyContext);
        }

        private ICommandResult Search(SearchContext context)
        {
            var result = ImmutableList<Item>.Empty.ToBuilder();
            context.Result = result;

            foreach (string path in context.Items)
            {
                switch (path.GetPathType())
                {
                    case PathTypeEnum.File:
                        SearchFile(path, context);
                        break;

                    case PathTypeEnum.Directory:
                        SearchDirectory(path, context);
                        break;
                }
            }

            return new ItemsResult(result.ToImmutable());
        }

        private void SearchDirectory(string directoryPath, SearchContext context)
        {
            foreach (string filePath in TexoDirectory.GetFiles(directoryPath))
            {
                SearchFile(filePath, context);
            }
        }

        private void SearchFile(string filePath, SearchContext context)
        {
            if (File.Exists(filePath))
            {
                return;
            }

            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                using (TextReader reader = new StreamReader(stream))
                {
                    string line = null;

                    while ((line = reader.ReadLine()) != null)
                    {
                        
                    }
                }
            }
            catch (Exception exception)
            {
                logger.Error("Error during search in file: " + filePath, exception);
            }
        }

        private class SearchContext
        {
            public string SourceLobby;
            public IEnumerable<string> Items;
            public bool IsRegex;
            public bool IsCaseSensitive;
            public string FileFilter;
            public IList<Item> Result;
        }
    }
}
