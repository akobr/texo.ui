using StrongBeaver.Core.Services;

namespace BeaverSoft.Texo.Core.Environment
{
    public class VariableUpdatedMessage : ServiceMessage, IVariableUpdatedMessage
    {
        public VariableUpdatedMessage(string name, string newValue, string oldValue)
        {
            Name = name;
            NewValue = newValue;
            OldValue = oldValue;
        }

        public string Name { get; }

        public string NewValue { get; }

        public string OldValue { get; }
    }
}
