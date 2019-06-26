namespace Commands.CodeBaseSearch.Conditions
{
    public class AndCondition<TValue> : BaseAggregatedCondition<TValue>
    {
        public AndCondition()
        {
            // no operation
        }

        public AndCondition(params ICondition<TValue>[] conditions)
            : base(conditions)
        {
            // no operation
        }

        public override bool IsMet(TValue value)
        {
            foreach (ICondition<TValue> condition in Conditions)
            {
                if (!condition.IsMet(value))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
