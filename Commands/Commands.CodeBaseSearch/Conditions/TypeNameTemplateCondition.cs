using Commands.CodeBaseSearch.Model;

namespace Commands.CodeBaseSearch.Conditions
{
    public class TypeNameTemplateCondition : BaseTemplateCondition<ISubject>
    {
        public override bool IsMet(ISubject value)
        {
            if (value.Type != SubjectTypeEnum.Type)
            {
                return false;
            }

            return regex.Value.IsMatch(value.Name);
        }
    }
}
