using System;

namespace BeaverSoft.Texo.Core.Extensibility.Attributes
{
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Method,
        Inherited = false,
        AllowMultiple = false)]
    public class OptionAttribute : Attribute
    {
        public OptionAttribute(string optionKey)
        {
            OptionKey = optionKey;
        }

        public string OptionKey { get; }

        public string Representations { get; set; }
    }
}
