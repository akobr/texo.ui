using System.Collections.Generic;
using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Inputting
{
    public partial class ParsedInput
    {
        public ParsedInput(string rawInput, IEnumerable<string> tokens)
        {
            RawInput = rawInput;
            Tokens = ImmutableList<string>.Empty.AddRange(tokens);
        }

        private ParsedInput()
        {
            RawInput = string.Empty;
            Tokens = ImmutableList<string>.Empty;
        }

        private ParsedInput(Builder builder)
        {
            RawInput = builder.RawInput;
            Tokens = builder.Tokens.ToImmutable();
        }

        public string RawInput { get; }

        public ImmutableList<string> Tokens { get; }

        public bool HasMultipleTokens => Tokens?.Count > 1;

        public static ParsedInput Empty { get; } = new ParsedInput();

        public Builder ToBuilder()
        {
            return new Builder(this);
        }
    }
}
