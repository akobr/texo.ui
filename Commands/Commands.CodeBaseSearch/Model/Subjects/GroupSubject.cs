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

        public override void WriteToMarkdown(MarkdownBuilder builder, int intent)
        {
            builder.Bullet(intent);
            builder.Bold($"{Name}");
            builder.Write(" ");
            builder.Italic($"(group)");

            int intentForChild = intent + 1;

            foreach (ISubject child in Children)
            {
                child.WriteToMarkdown(builder, intentForChild);
            }
        }

        public void AddChild(ISubject child)
        {
            SetChildren(Children.Add(child));
        }
    }
}
