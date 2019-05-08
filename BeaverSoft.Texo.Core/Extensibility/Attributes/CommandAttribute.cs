using System;

namespace BeaverSoft.Texo.Core.Extensibility.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class CommandAttribute : Attribute
    {
        public CommandAttribute(string commandKey)
        {
            CommandKey = commandKey;
        }

        public string CommandKey { get; }

        public bool IsDefault { get; set; }

        public string Representations { get; set; }
    }
}
