using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Model.Configuration
{
    public interface IInputStatement
    {
        string Key { get; }

        IImmutableList<string> Representations { get; }

        IImmutableList<IParameter> Parameters { get; }

        IDocumentation Documentation { get; }
    }
}