using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text.RegularExpressions;
using BeaverSoft.Texo.Commands.FileManager.Stage;
using BeaverSoft.Texo.Core.Actions;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Model.Text;
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
                SearchTerm = context.GetParameterValue(ApplyParameters.SEARCH_TERM),
                Items = paths,
                SourceLobby = stage.GetLobby(),
                IsRegex = context.HasOption(ApplyOptions.REGEX),
                IsCaseSensitive = context.HasOption(ApplyOptions.CASE_SENSITIVE)
            };

            return Search(copyContext);
        }

        private ICommandResult Search(SearchContext context)
        {
            var result = ImmutableList<ModeledItem>.Empty.ToBuilder();
            context.Result = result;
            context.FilesCount = 0;
            context.TotalMatchesCount = 0;

            try
            {
                if (context.IsRegex)
                {
                    context.Regex = new Regex(context.SearchTerm, RegexOptions.Compiled);
                }
            }
            catch (ArgumentException exception)
            {
                logger.Error("Content search: Error during regular expression parse.", exception);
                return new ErrorTextResult("Invalid regular expression.");
            }

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

            Document resultDocument = new Document(
                new Header("Search results"),
                new Paragraph($"Term: {context.SearchTerm}, {context.FilesCount} files searched, {context.TotalMatchesCount} results founded."));

            result.Add(new ModeledItem(resultDocument));
            return new ItemsResult<ModeledItem>(result.ToImmutable());
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
                context.LineNumber = 0;
                context.MatchesCount = 0;
                context.MatchResults = new Span();

                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                using (TextReader reader = new StreamReader(stream))
                {
                    while ((context.Line = reader.ReadLine()) != null)
                    {
                        context.LineNumber++;
                        SearchLine(context);
                    }
                }

                if (context.MatchesCount < 1)
                {
                    return;
                }

                Document fileDocoment = new Document(
                    new Header($"{filePath.GetFileNameOrDirectoryName()} ({context.MatchesCount})"),
                    new Paragraph(
                        new Core.Model.Text.Link(
                            filePath.GetFriendlyPath(context.SourceLobby),
                            ActionBuilder.PathOpenUri(filePath.GetFullPath()))),
                    new Paragraph(context.MatchResults));

                context.Result.Add(new ModeledItem(fileDocoment));
                context.FilesCount++;
                context.TotalMatchesCount += context.MatchesCount;
            }
            catch (Exception exception)
            {
                logger.Error("Error during search in file: " + filePath, exception);
            }

            context.FilesCount++;
        }

        private static void SearchLine(SearchContext context)
        {
            context.MatchResult = new SpanBuilder();
            context.MatchResult.Strong($"{context.LineNumber:0000}: ");
            int countBefore = context.MatchesCount;

            if (context.IsRegex)
            {
                RegexSearch(context);
            }
            else
            {
                IndexSearch(context);
            }

            if (context.MatchesCount <= countBefore)
            {
                return;
            }

            context.MatchResult.WriteLine();
            context.MatchResults = context.MatchResults.AddChild(context.MatchResult.Span);
        }

        private static void RegexSearch(SearchContext context)
        {
            int start = 0;

            foreach (Match match in context.Regex.Matches(context.Line))
            {
                if (start < match.Index)
                {
                    context.MatchResult.Write(context.Line.Substring(start, match.Index));
                }

                context.MatchesCount++;
                context.MatchResult.Marked(match.Value);
                start = match.Index + match.Length;
            }

            if (start < context.Line.Length)
            {
                context.MatchResult.Write(context.Line.Substring(start));
            }
        }

        private static void IndexSearch(SearchContext context)
        {
            int start = 0;

            StringComparison comparison = context.IsCaseSensitive
                ? StringComparison.Ordinal
                : StringComparison.OrdinalIgnoreCase;

            while (true)
            {
                int index = context.Line.IndexOf(context.Line, start, comparison);

                if (index < 0)
                {
                    break;
                }

                if (start < index)
                {
                    context.MatchResult.Write(context.Line.Substring(start, index));
                }

                context.MatchesCount++;
                context.MatchResult.Marked(context.Line.Substring(index, context.SearchTerm.Length));
                start = index + context.SearchTerm.Length;
            }

            if (start < context.Line.Length)
            {
                context.MatchResult.Write(context.Line.Substring(start));
            }
        }

        private class SearchContext
        {
            public IEnumerable<string> Items;
            public string SearchTerm;
            public string SourceLobby;
            public bool IsRegex;
            public bool IsCaseSensitive;

            public Regex Regex;
            public IList<ModeledItem> Result;

            public string Line;
            public int LineNumber;
            public ISpanBuilder MatchResult;
            public Span MatchResults;

            public int FilesCount;
            public int MatchesCount;
            public int TotalMatchesCount;
        }
    }
}
