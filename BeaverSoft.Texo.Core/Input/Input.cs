using System.Collections.Immutable;
using BeaverSoft.Texo.Core.Commands;

namespace BeaverSoft.Texo.Core.Input
{
    public class Input : IInput
    {
        private readonly CommandContext context;
        private readonly ImmutableList<IToken> tokens;

        private Input()
        {
            context = CommandContext.Empty;
            tokens = ImmutableList<IToken>.Empty;
            ParsedInput = Core.Input.ParsedInput.Empty;
        }

        private Input(Input toClone)
        {
            context = toClone.context;
            tokens = toClone.tokens;
            ParsedInput = toClone.ParsedInput;
        }

        private Input(Builder builder)
        {
            context = builder.Context.ToImmutable();
            tokens = builder.Tokens.ToImmutable();
            ParsedInput = builder.ParsedInput;
        }

        public ICommandContext Context => context;

        public IImmutableList<IToken> Tokens => tokens;

        public IParsedInput ParsedInput { get; private set; }

        public static Input Empty { get; } = new Input();

        public Input SetParsedInput(IParsedInput value)
        {
            return new Input(this)
            {
                ParsedInput = value
            };
        }

        public Builder ToBuilder()
        {
            return new Builder(this);
        }

        public static Input BuildUnrecognised(IParsedInput parsedInput)
        {
            return Empty.SetParsedInput(parsedInput);
        }

        public class Builder
        {
            internal Builder(Input input)
            {
                Context = input.context.ToBuilder();
                Tokens = input.tokens.ToBuilder();
                ParsedInput = input.ParsedInput;
            }

            public CommandContext.Builder Context { get; }

            public ImmutableList<IToken>.Builder Tokens { get; }

            public IParsedInput ParsedInput { get; set; }

            public Input ToImmutable()
            {
                return new Input(this);
            }
        }
    }
}
