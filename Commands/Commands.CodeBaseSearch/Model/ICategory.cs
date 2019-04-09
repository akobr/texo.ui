
using System.Collections.Immutable;

namespace Commands.CodeBaseSearch.Model
{
    public interface ICategory : ISearchable
    {
        IImmutableList<IGroup> Groups { get; }
    }
}