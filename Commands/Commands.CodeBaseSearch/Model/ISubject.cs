using BeaverSoft.Texo.Core.Markdown.Builder;

namespace Commands.CodeBaseSearch.Model
{
    public interface ISubject : ISearchable, ISearchTreeNode
    {
        SubjectTypeEnum Type { get; }

        void WriteToMarkdown(MarkdownBuilder builder, int intent = 0);
    }
}
