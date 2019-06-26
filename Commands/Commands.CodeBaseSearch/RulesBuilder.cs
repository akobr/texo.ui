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
                    Condition = new BaseTypeNameTemplateCondition() { Template = ".*ActionService" }
                },
                new Rule()
                {
                    CategoryName = "services",
                    CategoryCharacter = 's',
                    Condition = new OrCondition<ISubject>(
                        new BaseTypeNameTemplateCondition() { Template = "IService" },
                        new BaseTypeNameTemplateCondition() { Template = "Service" },
                        new BaseTypeNameTemplateCondition() { Template = "BaseService" })
                },
                new Rule()
                {
                    CategoryName = "update-services",
                    CategoryCharacter = 'u',
                    Condition = new BaseTypeNameTemplateCondition() { Template = ".*UpdateService" }
                },
                new Rule()
                {
                    CategoryName = "widgets",
                    CategoryCharacter = 'w',
                    Grouping = new NameTemplateGroupingStrategy() { Template = "(?<group>.+)Widget" },
                    Condition = new OrCondition<ISubject>(
                        new TypeNameTemplateCondition() { Template = ".+WidgetMediator" },
                        new TypeNameTemplateCondition() { Template = ".+WidgetState" },
                        new TypeNameTemplateCondition() { Template = ".+WidgetConfiguration" },
                        new TypeNameTemplateCondition() { Template = ".+WidgetPresenter" })
                },
                new Rule()
                {
                    CategoryName = "components",
                    CategoryCharacter = 'c',
                    Condition = new OrCondition<ISubject>(
                        new BaseTypeNameTemplateCondition() { Template = ".*Component" },
                        new TypeNameTemplateCondition() { Template = ".*TemplateItem" },
                        new TypeNameTemplateCondition() { Template = ".*(Template)?ItemCollection" })
                }
            };
        }
    }
}
