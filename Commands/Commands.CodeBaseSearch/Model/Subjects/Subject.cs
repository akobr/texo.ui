using System.Collections.Immutable;
using BeaverSoft.Texo.Core.Markdown.Builder;

namespace Commands.CodeBaseSearch.Model.Subjects
{
    public abstract class Subject : Searchable, ISubject
    {
        protected Subject(SubjectTypeEnum type, string name, IKeywordBuilder keywordBuilder)
            : base(name, keywordBuilder)
        {
            Type = type;
            Children = ImmutableList<ISubject>.Empty;
        }

        protected Subject(SubjectTypeEnum type, string name)
            : base(name)
        {
            Type = type;
            Children = ImmutableList<ISubject>.Empty;
        }

        public SubjectTypeEnum Type { get; }

        public ISubject Parent { get; protected set; }

        public IImmutableList<ISubject> Children { get; protected set; }

        public abstract void WriteToMarkdown(MarkdownBuilder builder, int intent);

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
