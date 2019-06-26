using Commands.CodeBaseSearch.Extensions;
using Commands.CodeBaseSearch.Model;
using Commands.CodeBaseSearch.Model.Subjects;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Commands.CodeBaseSearch.Conditions
{
    public class AttributeNameTemplateCondition : BaseTemplateCondition<ISubject>
    {
        public override bool IsMet(ISubject value)
        {
            if (value.Type != SubjectTypeEnum.Type)
            {
                return false;
            }

            switch (value)
            {
                case ClassTypeSubject classType:
                    return CheckBaseType(classType.ClassDeclaration.AttributeLists);

                case InterfaceTypeSubject interfaceType:
                    return CheckBaseType(interfaceType.InterfaceDeclaration.AttributeLists);

                case StructTypeSubject structType:
                    return CheckBaseType(structType.StructDeclaration.AttributeLists);

                default:
                    return false;
            }
        }

        private bool CheckBaseType(SyntaxList<AttributeListSyntax> attributeList)
        {
            foreach (AttributeListSyntax attributeListSyntax in attributeList)
                foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
                {
                    if (regex.Value.IsMatch(attributeSyntax.Name.GetTypeName()))
                    {
                        return true;
                    }
                }

            return false;
        }
    }
}
