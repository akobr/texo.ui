using System.Collections.Immutable;

namespace Commands.CodeBaseSearch.Model
{
    public interface ISearchable
    {
        string Name { get; }

        ImmutableList<string> Keywords { get; }
    }
}
