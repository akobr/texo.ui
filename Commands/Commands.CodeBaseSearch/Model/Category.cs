using System;
using System.Collections.Immutable;

namespace Commands.CodeBaseSearch.Model
{
    public class Category : Searchable, ICategory
    {
        public Category(string name, IImmutableList<IGroup> items)
            : base(name)
        {
            Groups = items;
        }

        public IImmutableList<IGroup> Groups { get; }
    }
}
