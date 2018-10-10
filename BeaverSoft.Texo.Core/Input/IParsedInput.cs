using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Input
{
    public interface IParsedInput
    {
        string RawInput { get; }

        IImmutableList<string> Tokens { get; }
    }
}
