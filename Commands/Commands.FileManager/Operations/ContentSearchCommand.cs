using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text.RegularExpressions;
using BeaverSoft.Texo.Commands.FileManager.Stage;
using BeaverSoft.Texo.Commands.FileManager.Stash;
using BeaverSoft.Texo.Core.Actions;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Markdown;
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
        private readonly IStashService stashes;
        private readonly ILogService logger;

        public ContentSearchCommand(IStageService stage, IStashService stashes, ILogService logger)
        {
            this.stage = stage ?? throw new ArgumentNullException(nameof(stage));
            this.stashes = stashes ?? throw new ArgumentNullException(nameof(stashes));
            this.logger = logger;
        }

        public ICommandResult Execute(CommandContext context)
        {
            IOperationSource source = context.GetOperationSource(stage, stashes);

            if (source.IsNullOrEmpty())
            {
                return new TextResult("The stage is empty.");
            }

            SearchContext searchContext = new SearchContext
            {
                SearchTerm = context.GetParameterValue(ApplyParameters.SEARCH_TERM),
                Items = source.GetPaths(),
                SourceLobby = source.GetLobby(),
                IsRegex = context.HasOption(ApplyOptions.REGEX),
                IsCaseSensitive = context.HasOption(ApplyOptions.CASE_SENSITIVE)
            };

            if (string.IsNullOrEmpty(searchContext.SearchTerm))
            {
                return new TextResult("Empty search term.");
            }

            return Search(searchContext);
        }

        private ICommandResult Search(SearchContext context)
        {
            var result = ImmutableList<ModeledItem>.Empty.ToBuilder();
            context.Result = result;
            context.FilesCount = 0;
            context.PositiveFilesCount = 0;
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
                new Paragraph($"Term: {context.SearchTerm}, {context.FilesCount} file(s) searched, {context.TotalMatchesCount} result(s) founded in {context.PositiveFilesCount} file(s)."));

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
            if (!File.Exists(filePath))
            {
                return;
            }

            try
            {
                context.FilesCount++;
                context.LineNumber = 0;
                context.MatchesCount = 0;
                context.MatchResults = new Span();

                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                using (TextReader reader = new StreamReader(stream))
                {
                    while ((context.Line = reader.ReadLine()) != null)
                    {
                        context.LineNumber++;
                        SearchLine(context, filePath);
                    }
                }

                if (context.MatchesCount < 1)
                {
                    return;
                }

                Document fileDocument = new Document(
                    new Header($"{filePath.GetFileNameOrDirectoryName()} ({context.MatchesCount})"),
                    new Paragraph(
                        new Core.Model.Text.Link(
                            filePath.GetFriendlyPath(context.SourceLobby),
                            ActionBuilder.PathOpenUri(filePath.GetFullPath()))),
                    new Paragraph(context.MatchResults));

                context.Result.Add(new ModeledItem(fileDocument));
                context.PositiveFilesCount++;
                context.TotalMatchesCount += context.MatchesCount;
            }
            catch (Exception exception)
            {
                logger.Error("Error during search in file: " + filePath, exception);
            }
        }

        private static void SearchLine(SearchContext context, string filePath)
        {
            if ((context.IsRegex && !context.Regex.IsMatch(context.Line))
                || context.Line.IndexOf(context.SearchTerm, 0, StringComparison.OrdinalIgnoreCase) < 0)
            {
                return;
            }

            context.MatchResult = new SpanBuilder();
            context.MatchResult.Write("- ");
            context.MatchResult.Link(
                $"{context.LineNumber:0000}",
                ActionBuilder.BuildActionUri(
                    ActionNames.PATH_OPEN,
                    new Dictionary<string, string>
                    {
                        { ActionParameters.PATH, filePath.GetFullPath() },
                        { "line", context.LineNumber.ToString() }
                    }));
            context.MatchResult.Write(": ");
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
                context.MatchesCount++;
                string prefix = start < match.Index
                    ? Markdown.Encode(context.Line.Substring(start, match.Index - start))
                    : null;
                string value = Markdown.Encode(match.Value);
                WriteSearchMatch(value, prefix, context);
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
                int index = context.Line.IndexOf(context.SearchTerm, start, comparison);
                
                if (index < 0)
                {
                    break;
                }

                context.MatchesCount++;
                string prefix = start < index
                    ? Markdown.Encode(context.Line.Substring(start, index - start))
                    : null;
                string value = Markdown.Encode(context.Line.Substring(index, context.SearchTerm.Length));
                WriteSearchMatch(value, prefix, context);
                start = index + context.SearchTerm.Length;
            }

            if (start < context.Line.Length)
            {
                context.MatchResult.Write(Markdown.Encode(context.Line.Substring(start)));
            }
        }

        private static void WriteSearchMatch(string value, string prefix, SearchContext context)
        {
            bool needSpace = false;

            if (prefix != null)
            {
                needSpace = prefix.EndsWith("\\");
                context.MatchResult.Write(prefix);
            }

            needSpace |= value.EndsWith("\\");

            if (needSpace)
            {
                context.MatchResult.Write(" ");
                context.MatchResult.Marked(value);
                context.MatchResult.Write(" ");
            }
            else
            {
                context.MatchResult.Marked(value);
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
            public int PositiveFilesCount;
            public int MatchesCount;
            public int TotalMatchesCount;
        }
    }
}
