using System;

namespace Commands.CodeBaseSearch.Conditions
{
    public class NotCondition<TValue> : ICondition<TValue>
    {
        private readonly ICondition<TValue> innerCondition;

        public NotCondition(ICondition<TValue> innerCondition)
        {
            this.innerCondition = innerCondition ?? throw new ArgumentNullException(nameof(innerCondition));
        }

        public bool IsMet(TValue value)
        {
            return !innerCondition.IsMet(value);
        }
    }
}
