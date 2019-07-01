using System.Collections.Immutable;

namespace Commands.CodeBaseSearch.Conditions
{
    public abstract class BaseAggregatedCondition<TValue> : ICondition<TValue>
    {
        private IImmutableList<ICondition<TValue>> conditions;

        public BaseAggregatedCondition()
        {
            conditions = ImmutableList<ICondition<TValue>>.Empty;
        }

        public BaseAggregatedCondition(params ICondition<TValue>[] conditions)
        {
            this.conditions = ImmutableList<ICondition<TValue>>.Empty.AddRange(conditions);
        }

        protected IImmutableList<ICondition<TValue>> Conditions => conditions;

        public abstract bool IsMet(TValue value);

        public void AddCondition(ICondition<TValue> condition)
        {
            conditions = conditions.Add(condition);
        }
    }
}
