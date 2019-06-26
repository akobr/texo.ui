using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
            RegisterQueryMethod(CodeBaseSearchConstants.QUERY_LIST, List);
            RegisterQueryMethod(CodeBaseSearchConstants.QUERY_CATEGORIES, Categories);
        }

        private async Task<ICommandResult> Search(CommandContext context)
        {
            MarkdownBuilder builder = new MarkdownBuilder();
            IImmutableList<ISubject> results = await search.SearchAsync(
                new SearchContext()
                {
                    SearchTerm = string.Join(" ", context.GetParameterValues(CodeBaseSearchConstants.PARAMETER_TERM)),
                    Categories = new HashSet<string>()
                });

            foreach (ISubject item in results)
            {
                item.WriteToMarkdown(builder);
            }

            return new MarkdownResult(builder.ToString());
        }

        private Task<ICommandResult> List(CommandContext context)
        {
            throw new NotImplementedException();
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
