using System;
using System.Text.RegularExpressions;

namespace Commands.CodeBaseSearch.Model.GroupingStrategies
{
    public class NameTemplateGroupingStrategy : IGroupingStrategy
    {
        private readonly Lazy<Regex> regex;

        public NameTemplateGroupingStrategy()
        {
            regex = new Lazy<Regex>(() => new Regex(Template), true);
        }

        public string Template { get; set; }

        public string GetGroup(ISubject subject)
        {
            Match match = regex.Value.Match(subject.Name);

            if (!match.Success)
            {
                return null;
            }

            Group group = match.Groups["group"];
            string groupName = group.Success
                ? group.Value
                : match.Value;

            if (groupName.Length > 1
                && groupName[0] == 'I'
                && char.IsUpper(groupName[1]))
            {
                groupName = groupName.Substring(1);
            }

            return groupName;
        }
    }
}
