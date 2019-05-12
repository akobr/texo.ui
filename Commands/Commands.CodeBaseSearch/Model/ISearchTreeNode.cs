using System.Collections.Immutable;

namespace Commands.CodeBaseSearch.Model
{
    public interface ISearchTreeNode
    {
        ISubject Parent { get; }

        IImmutableList<ISubject> Children { get; }
    }
}