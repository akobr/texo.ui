using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Configuration
{
    public sealed partial class Query : InputStatement
    {
        private ImmutableList<Query> queries;
        private ImmutableList<Option> options;
        private string defaultQueryKey;

        private Query()
        {
            queries = ImmutableList<Query>.Empty;
            options = ImmutableList<Option>.Empty;
        }

        private Query(Query toClone)
            : base(toClone)
        {
            queries = toClone.queries;
            options = toClone.options;
            defaultQueryKey = toClone.defaultQueryKey;
        }

        private Query(Builder builder)
            : base(builder)
        {
            queries = builder.Queries.ToImmutable();
            options = builder.Options.ToImmutable();
            defaultQueryKey = builder.DefaultQueryKey;
        }

        public ImmutableList<Query> Queries => queries;

        public ImmutableList<Option> Options => options;

        public string DefaultQueryKey => defaultQueryKey;

        public Query SetQueries(ImmutableList<Query> value)
        {
            return new Query(this)
            {
                queries = value
            };
        }

        public Query SetOptions(ImmutableList<Option> value)
        {
            return new Query(this)
            {
                options = value
            };
        }

        public Query SetDefaultQuery(string value)
        {
            return new Query(this)
            {
                defaultQueryKey = value
            };
        }

        public Query SetKey(string value)
        {
            return SetKey(new Query(this), value);
        }

        public Query SetRepresentations(ImmutableList<string> value)
        {
            return SetRepresentations(new Query(this), value);
        }

        public Query SetParameters(ImmutableList<Parameter> value)
        {
            return SetParameters(new Query(this), value);
        }

        public Query SetDocumentation(Documentation value)
        {
            return SetDocumentation(new Query(this), value);
        }

        public Builder ToBuilder()
        {
            return new Builder(this);
        }
    }
}
