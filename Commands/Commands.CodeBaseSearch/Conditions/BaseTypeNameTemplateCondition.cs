using Commands.CodeBaseSearch.Extensions;
using Commands.CodeBaseSearch.Model;
using Commands.CodeBaseSearch.Model.Subjects;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Commands.CodeBaseSearch.Conditions
{
    public class BaseTypeNameTemplateCondition : BaseTemplateCondition<ISubject>
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
                    return CheckBaseType(classType.ClassDeclaration.BaseList);

                case InterfaceTypeSubject interfaceType:
                    return CheckBaseType(interfaceType.InterfaceDeclaration.BaseList);

                case StructTypeSubject structType:
                    return CheckBaseType(structType.StructDeclaration.BaseList);

                default:
                    return false;
            }
        }

        private bool CheckBaseType(BaseListSyntax baseTypeList)
        {
            if (baseTypeList == null)
            {
                return false;
            }

            foreach (BaseTypeSyntax baseType in baseTypeList.Types)
            {
                if (baseType.Type is NameSyntax nameSyntax &&
                    regex.Value.IsMatch(nameSyntax.GetTypeName()))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
