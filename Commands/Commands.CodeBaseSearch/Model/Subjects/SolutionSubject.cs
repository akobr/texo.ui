using BeaverSoft.Texo.Core.Markdown.Builder;

namespace Commands.CodeBaseSearch.Model.Subjects
{
    public class SolutionSubject : Subject
    {
        public SolutionSubject(string name)
            : base(SubjectTypeEnum.Solution, name)
        {
            // no operation
        }

        public override void WriteToMarkdown(MarkdownBuilder builder)
        {
            builder.Bullet();
            builder.Bold($"SOLUTION: {Name}");
        }
    }
}
