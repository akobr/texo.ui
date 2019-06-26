namespace Commands.CodeBaseSearch.Model
{
    public class Rule : IRule
    {
        public string CategoryName { get; set; }

        public char CategoryCharacter { get; set; }

        public IGroupingStrategy Grouping { get; set; }

        public ICondition<ISubject> Condition { get; set; }
    }
}
