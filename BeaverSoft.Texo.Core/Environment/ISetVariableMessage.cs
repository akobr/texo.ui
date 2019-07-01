using StrongBeaver.Core.Services;

namespace BeaverSoft.Texo.Core.Environment
{
    public interface ISetVariableMessage : IServiceMessage
    {
        string Name { get; }

        string Value { get; }
    }
}
