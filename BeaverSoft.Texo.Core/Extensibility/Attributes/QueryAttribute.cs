using System;

namespace BeaverSoft.Texo.Core.Extensibility.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class QueryAttribute : Attribute
    {
        public QueryAttribute(string queryKey)
        {
            QueryKey = queryKey;
        }

        public string QueryKey { get; }

        public string Representations { get; set; }

        public bool IsDefault { get; set; }
    }
}
