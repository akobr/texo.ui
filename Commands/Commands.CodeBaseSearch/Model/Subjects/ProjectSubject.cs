using BeaverSoft.Texo.Core.Actions;
using BeaverSoft.Texo.Core.Markdown.Builder;
using Microsoft.CodeAnalysis;

namespace Commands.CodeBaseSearch.Model.Subjects
{
    public class ProjectSubject : Subject
    {
        private readonly Project project;

        public ProjectSubject(Project project)
            : base(SubjectTypeEnum.Project, project.Name)
        {
            this.project = project;
        }

        public override void WriteToMarkdown(MarkdownBuilder builder)
        {
            builder.Bullet();
            builder.Write("*");
            builder.Link(Name, ActionBuilder.PathOpenUri(project.FilePath));
            builder.Write("*");
        }
    }
}
