using BeaverSoft.Texo.Core.Markdown.Builder;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Commands.CodeBaseSearch.Model.Subjects
{
    public abstract class TypeSubject : SyntaxNodeSubject
    {
        public TypeSubject(TypeDeclarationSyntax type)
            : base(SubjectTypeEnum.Type, type, type.Identifier.ValueText)
        {
            // no operation
        }

        public virtual bool IsClass => false;

        public virtual bool IsInterface => false;

        public virtual bool IsStruct => false;

        public override void WriteToMarkdown(MarkdownBuilder builder, int intent)
        {
            base.WriteToMarkdown(builder, intent);
            builder.Write(" ");

            if (IsClass)
            {
                builder.Italic("(class)");
            }
            else if (IsInterface)
            {
                builder.Italic("(interface)");
            }
            else if (IsStruct)
            {
                builder.Italic("(struct)");
            }
        }
    }

    public class ClassTypeSubject : TypeSubject
    {
        public ClassTypeSubject(ClassDeclarationSyntax type)
            : base(type)
        {
            ClassDeclaration = type;
        }

        public override bool IsClass => true;

        public ClassDeclarationSyntax ClassDeclaration { get; }
    }

    public class InterfaceTypeSubject : TypeSubject
    {
        public InterfaceTypeSubject(InterfaceDeclarationSyntax type)
            : base(type)
        {
            InterfaceDeclaration = type;
        }

        public override bool IsInterface => true;

        public InterfaceDeclarationSyntax InterfaceDeclaration { get; }
    }

    public class StructTypeSubject : TypeSubject
    {
        public StructTypeSubject(StructDeclarationSyntax type)
            : base(type)
        {
            StructDeclaration = type;
        }

        public override bool IsStruct => true;

        public StructDeclarationSyntax StructDeclaration { get; }
    }
}
