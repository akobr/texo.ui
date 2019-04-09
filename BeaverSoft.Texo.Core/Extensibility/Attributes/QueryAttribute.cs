using System;

namespace BeaverSoft.Texo.Core.Extensibility.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class QueryAttribute : Attribute
    {
        public QueryAttribute(string queryKey)
        {
            QueryKey = queryKey;
        }

        public string QueryKey { get; }
    }
}
