using System.Collections.Immutable;
using Commands.CodeBaseSearch.Model.KeywordBuilders;

namespace Commands.CodeBaseSearch.Model
{
    public abstract class Searchable : ISearchable
    {
        public Searchable(string name)
            : this(name, new NameKeywordBuilder(name))
        {
            // no operation
        }

        public Searchable(string name, IKeywordBuilder keywordBuilder)
        {
            Name = name;
            Keywords = keywordBuilder.Build();
        }

        public string Name { get; protected set; }

        public IImmutableList<string> Keywords { get; protected set; }
    }
}
