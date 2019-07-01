namespace Commands.CodeBaseSearch.Conditions
{
    public class OrCondition<TValue> : BaseAggregatedCondition<TValue>
    {
        public OrCondition()
        {
            // no operation
        }

        public OrCondition(params ICondition<TValue>[] conditions)
            : base(conditions)
        {
            // no operation
        }

        public override bool IsMet(TValue subject)
        {
            foreach (ICondition<TValue> condition in Conditions)
            {
                if (condition.IsMet(subject))
                {
                    return true;
                }
            }

            return Conditions.Count < 1;
        }
    }
}
