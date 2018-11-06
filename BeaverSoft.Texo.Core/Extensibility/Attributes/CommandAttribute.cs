using System;

namespace BeaverSoft.Texo.Core.Extensibility.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class CommandAttribute : Attribute
    {
        public CommandAttribute(string commandKey)
        {
            Key = commandKey;
        }

        public string Key { get; }
    }
}
