using System.Collections.Generic;
using System.Collections.Immutable;
using Commands.CodeBaseSearch.Model.Subjects;
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

        // TODO: Make configuration how detailed the search is going to be (only types or members as well)
        public override void Visit(SyntaxNode node)
        {
            switch (node.Kind())
            {
                // case SyntaxKind.Attribute:
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.InterfaceDeclaration:
                case SyntaxKind.StructDeclaration:
                case SyntaxKind.MethodDeclaration: 
                case SyntaxKind.PropertyDeclaration:
                case SyntaxKind.CompilationUnit:
                case SyntaxKind.NamespaceDeclaration:
                    base.Visit(node);
                    break;
            }
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var typeSubject = new ClassTypeSubject(node);
            typeSubject.SetParent(fileSubject);

            typeStack.Push(typeSubject);
            base.VisitClassDeclaration(node);
            typesList.Add(typeStack.Pop());
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            var typeSubject = new InterfaceTypeSubject(node);
            typeSubject.SetParent(fileSubject);

            typeStack.Push(typeSubject);
            base.VisitInterfaceDeclaration(node);
            typesList.Add(typeStack.Pop());
            
        }

        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            var typeSubject = new StructTypeSubject(node);
            typeSubject.SetParent(fileSubject);

            typeStack.Push(typeSubject);
            base.VisitStructDeclaration(node);
            typesList.Add(typeStack.Pop());
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            var methodSubject = new MemberSubject(node, node.Identifier.ValueText);
            var parentType = typeStack.Peek();
            parentType.SetChildren(parentType.Children.Add(methodSubject));
            methodSubject.SetParent(parentType);
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            var propertySubject = new MemberSubject(node, node.Identifier.ValueText);
            var parentType = typeStack.Peek();
            parentType.SetChildren(parentType.Children.Add(propertySubject));
            propertySubject.SetParent(parentType);
        }
    }
}
