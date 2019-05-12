using System.Collections.Immutable;

namespace Commands.CodeBaseSearch.Model
{
    public abstract class Searchable : ISearchable
    {
        public Searchable()
        {
            // no operation
        }

        public Searchable(string name)
        {
            Name = name;
            Keywords = new NameKeywordBuilder(name).Build();
        }

        public string Name { get; protected set; }

        public IImmutableList<string> Keywords { get; protected set; }
    }
}
