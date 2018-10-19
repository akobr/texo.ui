using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Model.Configuration
{
    public sealed partial class Option : InputStatement
    {
        private Option()
            : base()
        {
            // no operation
        }

        private Option(Option toClone)
            : base(toClone)
        {
            // no operation
        }

        private Option(Builder builder)
            : base(builder)
        {
            // no operation
        }

        public Option SetKey(string value)
        {
            return SetKey(new Option(this), value);
        }

        public Option SetRepresentations(ImmutableList<string> value)
        {
            return SetRepresentations(new Option(this), value);
        }

        public Option SetParameters(ImmutableList<Parameter> value)
        {
            return SetParameters(new Option(this), value);
        }

        public Option SetDocumentation(Documentation value)
        {
            return SetDocumentation(new Option(this), value);
        }

        public Builder ToBuilder()
        {
            return new Builder(this);
        }


    }
}
