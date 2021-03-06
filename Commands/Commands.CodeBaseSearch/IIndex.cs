using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Commands.CodeBaseSearch.Model
{
    public interface IIndex
    {
        Task LoadAsync(IReporter reporter);

        Task<IImmutableList<ISubject>> SearchAsync(SearchContext context);

        IEnumerable<ICategory> GetCategories();
    }
}