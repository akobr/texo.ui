using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Commands.CodeBaseSearch.Conditions
{
    public abstract class BaseTemplateCondition<TValue> : ICondition<TValue>
    {
        protected readonly Lazy<Regex> regex;

        public BaseTemplateCondition()
        {
            regex = new Lazy<Regex>(() => new Regex(Template), false);
        }

        public string Template { get; set; }

        public abstract bool IsMet(TValue value);
    }
}
