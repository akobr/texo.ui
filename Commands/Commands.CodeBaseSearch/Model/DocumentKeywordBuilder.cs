using System.Collections.Immutable;
using Humanizer;
using Microsoft.CodeAnalysis;

namespace Commands.CodeBaseSearch.Model
{
    class DocumentKeywordBuilder : IKeywordBuilder
    {
        private readonly Document file;

        public DocumentKeywordBuilder(Document file)
        {
            this.file = file;
        }

        public IImmutableList<string> Build()
        {
            var keywordBuilder = ImmutableList<string>.Empty.ToBuilder();
            keywordBuilder.AddRange(file.Name.Humanize(LetterCasing.LowerCase).Split(' '));
            keywordBuilder.AddRange(file.Folders);
            return keywordBuilder.ToImmutable();
        }
    }
}
