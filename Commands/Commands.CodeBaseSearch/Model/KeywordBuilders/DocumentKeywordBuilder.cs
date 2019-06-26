using System.Collections.Immutable;
using System.IO;
using Humanizer;
using Microsoft.CodeAnalysis;

namespace Commands.CodeBaseSearch.Model.KeywordBuilders
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
            string simpleName = /*Path.GetFileNameWithoutExtension(*/file.Name/*)*/;
            var keywordBuilder = ImmutableList<string>.Empty.ToBuilder();
            //keywordBuilder.Add(simpleName);
            keywordBuilder.AddRange(simpleName.Humanize(LetterCasing.LowerCase).Split(' '));
            keywordBuilder.AddRange(file.Folders);
            return keywordBuilder.ToImmutable();
        }
    }
}
