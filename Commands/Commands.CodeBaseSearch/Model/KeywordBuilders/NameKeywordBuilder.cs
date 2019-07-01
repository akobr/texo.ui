using System.Collections.Immutable;
using Humanizer;

namespace Commands.CodeBaseSearch.Model.KeywordBuilders
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
            var keywordBuilder = ImmutableList<string>.Empty.ToBuilder();
            //keywordBuilder.Add(name);
            keywordBuilder.AddRange(name.Humanize(LetterCasing.LowerCase).Split(' '));
            return keywordBuilder.ToImmutable();
        }
    }
}
