using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Commands.CodeBaseSearch.Model;

namespace Commands.CodeBaseSearch
{
    public class CodeBaseSearchService : ICodeBaseSearchService
    {
        private readonly ISolutionOpenStrategy openStrategy;
        private IIndex index;

        public CodeBaseSearchService(ISolutionOpenStrategy openStrategy)
        {
            this.openStrategy = openStrategy ?? throw new ArgumentNullException(nameof(openStrategy));
        }

        public Task LoadAsync()
        {
            return LoadAsync(null);
        }

        public Task LoadAsync(IReporter reporter)
        {
            index = new SolutionIndex(openStrategy);
            return index.LoadAsync(reporter);
        }

        public Task<IImmutableList<ISubject>> SearchAsync(SearchContext context)
        {
            if (index == null)
            {
                return Task.FromResult<IImmutableList<ISubject>>(ImmutableList<ISubject>.Empty);
            }

            return index.SearchAsync(context);
        }

        public IEnumerable<ICategory> GetCategories()
        {
            if (index == null)
            {
                return Enumerable.Empty<ICategory>();
            }

            return index.GetCategories();
        }
    }
}
