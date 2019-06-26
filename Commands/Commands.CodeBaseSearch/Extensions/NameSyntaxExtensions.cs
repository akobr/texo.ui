using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Commands.CodeBaseSearch.Extensions
{
    public static class NameSyntaxExtensions
    {
        public static string GetTypeName(this NameSyntax nameSyntax)
        {
            switch (nameSyntax)
            {
                case SimpleNameSyntax simple:
                    return simple.Identifier.ValueText;

                case QualifiedNameSyntax qualified:
                    return qualified.Right.Identifier.ValueText;

                case AliasQualifiedNameSyntax alias:
                    return alias.Alias.Identifier.ValueText;

                default:
                    return nameSyntax.ToString();
            }
        }
    }
}
