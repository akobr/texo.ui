using System;

namespace BeaverSoft.Texo.Core.Extensibility.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class QueryAttribute : Attribute
    {
        public QueryAttribute(string queryKey)
        {
            Key = queryKey;
        }

        public string Key { get; }
    }
}
