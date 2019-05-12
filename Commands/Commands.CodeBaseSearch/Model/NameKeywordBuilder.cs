using System.Collections.Immutable;
using Humanizer;

namespace Commands.CodeBaseSearch.Model
{
    public class NameKeywordBuilder : IKeywordBuilder
    {
        private readonly string name;

        public NameKeywordBuilder(string name)
        {
            this.name = name;
        }

        public IImmutableList<string> Build()
        {
            return ImmutableList<string>.Empty.AddRange(name.Humanize(LetterCasing.LowerCase).Split(' '));
        }
    }
}
