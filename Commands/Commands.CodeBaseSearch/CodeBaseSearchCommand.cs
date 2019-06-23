using System;
using System.Collections.Immutable;
using System.Text;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Commands;
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

        private async Task<ICommandResult> Search(CommandContext context)
        {
            StringBuilder builder = new StringBuilder();
            IImmutableList<ISubject> results = await search.SearchAsync(
                new SearchContext()
                {
                    SearchTerm = string.Join(" ", context.GetParameterValues(CodeBaseSearchConstants.PARAMETER_TERM))
                });

            foreach (ISubject item in results)
            {
                builder.AppendLine($"{item.Name} ({item.Type})");
            }

            return new TextResult(builder.ToString());
        }

        private Task<ICommandResult> Categories(CommandContext context)
        {
            throw new NotImplementedException();
        }
    }
}
