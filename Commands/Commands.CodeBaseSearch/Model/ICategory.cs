
using System.Collections.Immutable;

namespace Commands.CodeBaseSearch.Model
{
    public interface ICategory : ISearchable
    {
        char Character { get; }

        ICondition<ISubject> Condition { get; }

        IGroupingStrategy Grouping { get; }

        IImmutableList<ISubject> Subjects { get; }
    }
}