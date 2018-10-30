using StrongBeaver.Core.Services;

namespace BeaverSoft.Texo.Core.Environment
{
    public interface IVariableUpdatedMessage : IServiceMessage
    {
        string Name { get; }

        string NewValue { get; }

        string OldValue { get; }
    }
}