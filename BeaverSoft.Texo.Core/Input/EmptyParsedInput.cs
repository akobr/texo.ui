using System.Collections.Immutable;
using BeaverSoft.Texo.Core.Commands;

namespace BeaverSoft.Texo.Core.Input
{
    public class EmptyParsedInput : IParsedInput
    {
        public string RawInput => string.Empty;

        public IImmutableList<string> Tokens => ImmutableList<string>.Empty;

        public string CommandKey => string.Empty;

        public ICommandContext CommandContext => null;
    }
}
