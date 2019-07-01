namespace Commands.CodeBaseSearch.Model
{
    public interface IRule
    {
        string CategoryName { get; }

        char CategoryCharacter { get; }

        IGroupingStrategy Grouping { get; }

        ICondition<ISubject> Condition { get; }
    }
}
