using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Configuration
{
    public partial class InputStatement
    {
        public abstract class BaseBuilder
        {
            protected BaseBuilder(InputStatement configuration)
            {
                Key = configuration.key;
                Representations = configuration.representations.ToBuilder();
                Parameters = configuration.parameters.ToBuilder();
                Documentation = configuration.documentation.ToBuilder();
            }

            public string Key { get; set; }

            public ImmutableList<string>.Builder Representations { get; set; }

            public ImmutableList<Parameter>.Builder Parameters { get; set; }

            public Documentation.Builder Documentation { get; set; }
        }
    }
}
