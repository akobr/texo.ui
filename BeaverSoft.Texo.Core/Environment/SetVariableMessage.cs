using StrongBeaver.Core.Services;

namespace BeaverSoft.Texo.Core.Environment
{
    public class SetVariableMessage : ServiceMessage, ISetVariableMessage
    {
        public SetVariableMessage(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }

        public string Value { get; }
    }
}
