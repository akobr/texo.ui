using System.Collections.Immutable;

namespace Commands.CodeBaseSearch.Model
{
    public interface IGroup : ISearchable
    {
        IImmutableList<ISubject> Items { get; }
    }
}