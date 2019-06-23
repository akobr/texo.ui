using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Commands.CodeBaseSearch.Model
{
    public class TypeAndMembersWalker : CSharpSyntaxWalker
    {
        private readonly ImmutableList<ISubject>.Builder typesList;
        private readonly Stack<Subject> typeStack;
        private readonly ISubject fileSubject;

        public TypeAndMembersWalker(ISubject fileSubject)
        {
            this.fileSubject = fileSubject;
            typesList = ImmutableList<ISubject>.Empty.ToBuilder();
            typeStack = new Stack<Subject>();
        }

        public IImmutableList<ISubject> GetTypeSubjects()
        {
            return typesList.ToImmutable();
        }

        public override void Visit(SyntaxNode node)
        {
            switch (node.Kind())
            {
                // case SyntaxKind.Attribute:
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.MethodDeclaration:
                case SyntaxKind.CompilationUnit:
                case SyntaxKind.NamespaceDeclaration:
                    base.Visit(node);
                    break;
            }
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var typeSubject = new Subject(node.Identifier.ValueText, SubjectTypeEnum.Type);
            typeSubject.SetParent(fileSubject);
            typeSubject.SetChildren(ImmutableList<ISubject>.Empty);
            typeStack.Push(typeSubject);

            base.VisitClassDeclaration(node);

            typesList.Add(typeStack.Pop());
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            var methodSubject = new Subject(node.Identifier.ValueText, SubjectTypeEnum.Member);
            var parentType = typeStack.Peek();
            parentType.SetChildren(parentType.Children.Add(methodSubject));
            methodSubject.SetParent(parentType);

            // base.VisitMethodDeclaration(node);
        }
    }
}
