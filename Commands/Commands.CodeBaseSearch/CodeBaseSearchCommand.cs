using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Markdown.Builder;
using BeaverSoft.Texo.Core.Result;
using Commands.CodeBaseSearch.Model;

namespace Commands.CodeBaseSearch
{
    public class CodeBaseSearchCommand : InlineIntersectionAsyncCommand
    {
        private readonly ICodeBaseSearchService search;

        public CodeBaseSearchCommand(ICodeBaseSearchService search)
        {
            this.search = search ?? throw new ArgumentNullException(nameof(search));

            RegisterQueryMethod(CodeBaseSearchConstants.QUERY_SEARCH, Search);
            RegisterQueryMethod(CodeBaseSearchConstants.QUERY_CATEGORIES, Categories);
        }

        // TODO: refactor this mess
        private async Task<ICommandResult> Search(CommandContext context)
        {
            MarkdownBuilder builder = new MarkdownBuilder();

            var args = context.GetParameterValues(CodeBaseSearchConstants.PARAMETER_TERM).ToList();
            HashSet<string> categories = new HashSet<string>();
            var currentCategories = search.GetCategories().ToList();

            for (int i = 0; i < args.Count; i++)
            {
                string catCharacter = args[i];

                if (catCharacter.Length > 1)
                {
                    break;
                }

                ICategory category = currentCategories.FirstOrDefault(c => c.Character == catCharacter[0]);

                if (category == null)
                {
                    break;
                }

                categories.Add(category.Name);
            }

            IImmutableList<ISubject> results = await search.SearchAsync(
                new SearchContext()
                {
                    SearchTerm = string.Join(" ", args),
                    Categories = categories
                });

            if (results.Count < 1)
            {
                builder.Italic("No results");
                return new MarkdownResult(builder.ToString());
            }

            foreach (ISubject item in results)
            {
                item.WriteToMarkdown(builder);
            }

            return new MarkdownResult(builder.ToString());
        }

        private Task<ICommandResult> Categories(CommandContext context)
        {
            StringBuilder builder = new StringBuilder();

            foreach (ICategory category in search.GetCategories())
            {
                builder.AppendLine($"-{category.Character}--{category.Name} ({category.Subjects.Count})");
            }

            return Task.FromResult<ICommandResult>(new TextResult(builder.ToString()));
        }
    }
}
