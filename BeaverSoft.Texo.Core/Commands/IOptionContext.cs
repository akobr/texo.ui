using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Commands
{
    public interface IOptionContext
    {
        string Key { get; }

        IImmutableDictionary<string, IParameterContext> Parameters { get; }
    }
}