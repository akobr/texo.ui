using System;
using System.Text.RegularExpressions;

namespace Commands.CodeBaseSearch.Model.GroupingStrategies
{
    public class NameTemplateGroupingStrategy : IGroupingStrategy
    {
        private readonly Lazy<Regex> regex;

        public NameTemplateGroupingStrategy()
        {
            regex = new Lazy<Regex>(() => new Regex(Template), false);
        }

        public string Template { get; set; }

        public string GetGroup(ISubject subject)
        {
            Match match = regex.Value.Match(subject.Name);

            if (match.Success)
            {
                Group group = match.Groups["group"];
                if (group.Success)
                {
                    return group.Value;
                }

                return match.Value;
            }

            return null;
        }
    }
}
