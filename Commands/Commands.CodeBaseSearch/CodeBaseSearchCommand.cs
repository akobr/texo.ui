using System;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Commands;

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
            throw new NotImplementedException();
        }

        private Task<ICommandResult> Categories(CommandContext context)
        {
            throw new NotImplementedException();
        }
    }
}
