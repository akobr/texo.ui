using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Inputting
{
    public partial class ParsedInput
    {
        public class Builder
        {
            internal Builder(ParsedInput input)
            {
                RawInput = input.RawInput;
                Tokens = input.Tokens.ToBuilder();
            }

            public string RawInput { get; set; }

            public ImmutableList<string>.Builder Tokens { get; }

            public ParsedInput ToImmutable()
            {
                return new ParsedInput(this);
            }
        }
    }
}
