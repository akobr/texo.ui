using System.Collections.Immutable;

namespace Commands.CodeBaseSearch.Model
{
    public interface IKeywordBuilder
    {
        IImmutableList<string> Build();
    }
}