using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Commands.CodeBaseSearch.Model;

namespace Commands.CodeBaseSearch
{
    public interface ICodeBaseSearchService
    {
        Task LoadAsync();

        Task LoadAsync(IReporter reporter);

        Task<IImmutableList<ISubject>> SearchAsync(SearchContext context);

        IEnumerable<ICategory> GetCategories();
    }
}
