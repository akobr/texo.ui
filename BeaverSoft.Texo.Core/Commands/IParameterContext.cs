using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Commands
{
    public interface IParameterContext
    {
        string Key { get; }

        string GetValue();

        IImmutableList<string> GetValues();
    }
}