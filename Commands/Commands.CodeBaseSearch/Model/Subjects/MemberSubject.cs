using BeaverSoft.Texo.Core.Markdown.Builder;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Commands.CodeBaseSearch.Model.Subjects
{
    public class MemberSubject : SyntaxNodeSubject
    {
        public MemberSubject(MemberDeclarationSyntax member, string name)
            : base(SubjectTypeEnum.Member, member, name)
        {
            // no operation
        }

        public override void WriteToMarkdown(MarkdownBuilder builder, int intent)
        {
            builder.Bullet(intent);
            builder.Link($"{Parent.Name}.{Name}", GetLinkUrl());
            builder.Write(" ");

            if (syntaxNode is PropertyDeclarationSyntax)
            {
                builder.Italic("(property)");
            }
            else
            {
                builder.Italic("(method)");
            }
        }
    }
}
