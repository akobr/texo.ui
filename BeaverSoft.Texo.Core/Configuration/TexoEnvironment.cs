using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Configuration
{
    public partial class TexoEnvironment
    {
        private ImmutableDictionary<string, string> variables;

        private TexoEnvironment()
        {
            variables = ImmutableDictionary<string, string>.Empty;
        }

        private TexoEnvironment(TexoEnvironment toClone)
        {
            variables = toClone.variables;
        }

        private TexoEnvironment(Builder builder)
        {
            variables = builder.Variables.ToImmutable();
        }

        public ImmutableDictionary<string, string> Variables => variables;

        public TexoEnvironment SetVariables(ImmutableDictionary<string, string> value)
        {
            return new TexoEnvironment(this)
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
