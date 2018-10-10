using System.Collections.Immutable;
using BeaverSoft.Texo.Core.Commands;

namespace BeaverSoft.Texo.Core.Input
{
    public interface IInput
    {
        ICommandContext Context { get; }

        IImmutableList<IToken> Tokens { get; }

        IParsedInput ParsedInput { get; }
    }
}