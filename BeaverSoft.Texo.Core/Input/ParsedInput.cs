using System.Collections.Generic;
using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Input
{
    public partial class ParsedInput : IParsedInput
    {
        private readonly ImmutableList<string> tokens;

        public ParsedInput(string rawInput, IEnumerable<string> tokens)
        {
            RawInput = rawInput;
            this.tokens = ImmutableList<string>.Empty.AddRange(tokens);
        }

        private ParsedInput()
        {
            RawInput = string.Empty;
            tokens = ImmutableList<string>.Empty;
        }

        private ParsedInput(Builder builder)
        {
            RawInput = builder.RawInput;
            tokens = builder.Tokens.ToImmutable();
        }

        public string RawInput { get; }

        public IImmutableList<string> Tokens => tokens;

        public static ParsedInput Empty { get; } = new ParsedInput();

        public Builder ToBuilder()
        {
            return new Builder(this);
        }
    }
}
