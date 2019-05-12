using System;
using System.Collections.Immutable;

namespace Commands.CodeBaseSearch.Model
{
    public class Group : Searchable, IGroup
    {
        public Group(string name, IImmutableList<ISubject> items)
            : base(name)
        {
            Items = items;
        }

        public IImmutableList<ISubject> Items { get; }
    }
}
