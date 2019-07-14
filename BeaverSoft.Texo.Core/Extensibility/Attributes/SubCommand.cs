using System;

namespace BeaverSoft.Texo.Core.Extensibility.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class SubCommandAttribute : QueryAttribute
    {
        public SubCommandAttribute(string queryKey)
            : base(queryKey)
        {
            // no operation
        }

        public string Path { get; set; }
    }
}
