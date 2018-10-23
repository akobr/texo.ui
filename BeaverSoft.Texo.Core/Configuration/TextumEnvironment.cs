using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Configuration
{
    public partial class TextumEnvironment
    {
        private ImmutableDictionary<string, string> variables;

        private TextumEnvironment()
        {
            variables = ImmutableDictionary<string, string>.Empty;
        }

        private TextumEnvironment(TextumEnvironment toClone)
        {
            variables = toClone.variables;
        }

        private TextumEnvironment(Builder builder)
        {
            variables = builder.Variables.ToImmutable();
        }

        public ImmutableDictionary<string, string> Variables => variables;

        public TextumEnvironment SetVariables(ImmutableDictionary<string, string> value)
        {
            return new TextumEnvironment(this)
            {
                variables = value
            };
        }

        public Builder ToBuilder()
        {
            return new Builder(this);
        }
    }
}
