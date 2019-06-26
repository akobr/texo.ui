using System;
using System.Collections.Immutable;

namespace Commands.CodeBaseSearch.Model
{
    public class Category : Searchable, ICategory
    {
        private readonly object writeLock = new object();
        private IImmutableList<ISubject> subjects;
        private IImmutableDictionary<string, IImmutableSet<ISubject>> map;

        public Category(string name, char character)
            : base(name)
        {
            Character = character;
            subjects = ImmutableList<ISubject>.Empty;
            map = ImmutableDictionary<string, IImmutableSet<ISubject>>.Empty.WithComparers(StringComparer.OrdinalIgnoreCase);
        }

        public char Character { get; }

        public ICondition<ISubject> Condition { get; private set; }

        public IImmutableList<ISubject> Subjects => subjects;

        public IImmutableDictionary<string, IImmutableSet<ISubject>> Map => map;

        public void AddSubject(ISubject subject)
        {
            lock (writeLock)
            {
                subjects = subjects.Add(subject);

                foreach (string keyword in subject.Keywords)
                {
                    if (map.TryGetValue(keyword, out var keywordSet))
                    {
                        map = map.SetItem(keyword, keywordSet.Add(subject));
                    }
                    else
                    {
                        map = map.SetItem(keyword, ImmutableHashSet<ISubject>.Empty.Add(subject));
                    }
                }
            }
        }

        public void LinkCondition(ICondition<ISubject> condition)
        {
            Condition = condition;
        }
    }
}
