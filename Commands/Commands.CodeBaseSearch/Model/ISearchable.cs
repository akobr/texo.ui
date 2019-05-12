using System.Collections.Immutable;

namespace Commands.CodeBaseSearch.Model
{
    public interface ISearchable
    {
        string Name { get; }

        IImmutableList<string> Keywords { get; }
    }
}
