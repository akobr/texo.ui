using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Inputting
{
    public interface IParsedInput
    {
        string RawInput { get; }

        IImmutableList<string> Tokens { get; }
    }
}
