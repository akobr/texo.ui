using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Model.Configuration
{
    public partial class Query
    {
        public sealed class Builder : BaseBuilder
        {
            public Builder(Query configuration)
                : base(configuration)
            {
                Queries = configuration.queries.ToBuilder();
                Options = configuration.options.ToBuilder();
                DefaultQueryKey = configuration.defaultQueryKey;
            }

            public ImmutableList<IQuery>.Builder Queries { get; set; }

            public ImmutableList<IOption>.Builder Options { get; set; }

            public string DefaultQueryKey { get; set; }

            public Query ToImmutable()
            {
                return new Query(this);
            }
        }
    }
}