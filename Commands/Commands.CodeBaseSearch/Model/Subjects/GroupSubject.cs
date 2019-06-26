using BeaverSoft.Texo.Core.Markdown.Builder;

namespace Commands.CodeBaseSearch.Model.Subjects
{
    public class GroupSubject : Subject
    {
        public GroupSubject(string name)
            : base(SubjectTypeEnum.Group, name)
        {
            // no operation
        }

        public override void WriteToMarkdown(MarkdownBuilder builder)
        {
            builder.Bullet();
            builder.Bold($"GROUP: {Name}");
        }

        public void AddChild(ISubject child)
        {
            SetChildren(Children.Add(child));
        }
    }
}
