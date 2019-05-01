using System.Collections.Immutable;
using BeaverSoft.Texo.Core.Commands;

namespace BeaverSoft.Texo.Core.Inputting
{
    public interface IInput
    {
        CommandContext Context { get; }

        IImmutableList<IToken> Tokens { get; }

        IParsedInput ParsedInput { get; }
    }
}