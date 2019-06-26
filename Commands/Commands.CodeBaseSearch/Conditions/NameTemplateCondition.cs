namespace Commands.CodeBaseSearch.Conditions
{
    public class NameTemplateCondition : BaseTemplateCondition<string>
    {
        public override bool IsMet(string value)
        {
            return regex.Value.IsMatch(value);
        }
    }
}
