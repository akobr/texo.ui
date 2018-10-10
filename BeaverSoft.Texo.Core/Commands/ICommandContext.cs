using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Commands
{
    public interface ICommandContext
    {
        string Key { get; }

        bool IsValid { get; }

        IImmutableDictionary<string, IOptionContext> Options { get; }

        IImmutableDictionary<string, IParameterContext> Parameters { get; }
    }
}