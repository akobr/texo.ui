namespace Commands.CodeBaseSearch
{
    public interface ICondition<TValue>
    {
        bool IsMet(TValue value);
    }
}
