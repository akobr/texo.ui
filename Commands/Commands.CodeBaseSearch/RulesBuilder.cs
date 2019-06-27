using Commands.CodeBaseSearch.Conditions;
using Commands.CodeBaseSearch.Model;
using Commands.CodeBaseSearch.Model.GroupingStrategies;

namespace Commands.CodeBaseSearch
{
    public static class RulesBuilder
    {
        public static IRule[] Build()
        {
            return new[]
            {
                new Rule()
                {
                    CategoryName = "actions",
                    CategoryCharacter = 'a',
                    Condition = new BaseTypeNameTemplateCondition() { Template = "^.*ActionService$" }
                },
                new Rule()
                {
                    CategoryName = "services",
                    CategoryCharacter = 's',
                    Condition = new BaseTypeNameTemplateCondition() { Template = "^(I|Base)?Service$" }
                },
                new Rule()
                {
                    CategoryName = "update-services",
                    CategoryCharacter = 'u',
                    Condition = new BaseTypeNameTemplateCondition() { Template = "^.*UpdateService$" }
                },
                new Rule()
                {
                    CategoryName = "widgets",
                    CategoryCharacter = 'w',
                    Grouping = new NameTemplateGroupingStrategy() { Template = "(?<group>.+)Widget" },
                    Condition = new TypeNameTemplateCondition() { Template = "^.{2,}Widget(Mediator|Presenter|Configuration(Model)?|State|Model)$" }
                },
                new Rule()
                {
                    CategoryName = "components",
                    CategoryCharacter = 'c',
                    Condition = new OrCondition<ISubject>(
                        new TypeNameTemplateCondition() { Template = "^.*(Component|TemplateItem(Collection)?)$" },
                        new BaseTypeNameTemplateCondition() { Template = "^.*(Component|TemplateItem(Collection)?)$" })
                }
            };
        }
    }
}
