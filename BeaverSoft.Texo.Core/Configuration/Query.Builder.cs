using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Configuration
{
    public partial class Query
    {
        public sealed class Builder : BaseBuilder
        {
            internal Builder(Query configuration)
                : base(configuration)
            {
                Queries = configuration.queries.ToBuilder();
                Options = configuration.options.ToBuilder();
                DefaultQueryKey = configuration.defaultQueryKey;
            }

            public ImmutableList<Query>.Builder Queries { get; set; }

            public ImmutableList<Option>.Builder Options { get; set; }

            public string DefaultQueryKey { get; set; }

            public Query ToImmutable()
            {
                return new Query(this);
            }
        }
    }
}