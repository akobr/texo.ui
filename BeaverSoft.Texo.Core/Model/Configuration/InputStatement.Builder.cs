using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Model.Configuration
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
                Documentation = configuration.documentation;
            }

            public string Key { get; set; }

            public ImmutableList<string>.Builder Representations { get; set; }

            public ImmutableList<IParameter>.Builder Parameters { get; set; }

            public IDocumentation Documentation { get; set; }
        }
    }
}
