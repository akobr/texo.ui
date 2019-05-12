using System.Collections.Immutable;

namespace Commands.CodeBaseSearch.Model
{
    public class Subject : Searchable, ISubject
    {
        public Subject()
        {
            // no operation
        }

        public Subject(string name, SubjectTypeEnum type)
            : base(name)
        {
            Type = type;
        }

        public SubjectTypeEnum Type { get; }

        public ISubject Parent { get; protected set; }

        public IImmutableList<ISubject> Children { get; protected set; }

        internal void SetParent(ISubject parent)
        {
            Parent = parent;
        }

        internal void SetChildren(IImmutableList<ISubject> children)
        {
            Children = children;
        }
    }
}
