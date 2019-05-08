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

        public string Representations { get; set; }
    }
}
