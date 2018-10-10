using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Model.Configuration
{
    public sealed partial class Query : InputStatement, IQuery
    {
        private ImmutableList<IQuery> queries;
        private ImmutableList<IOption> options;
        private string defaultQueryKey;

        private Query()
        {
            queries = ImmutableList<IQuery>.Empty;
            options = ImmutableList<IOption>.Empty;
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

        public IImmutableList<IQuery> Queries => queries;

        public IImmutableList<IOption> Options => options;

        public string DefaultQueryKey => defaultQueryKey;

        public Query SetQueries(ImmutableList<IQuery> value)
        {
            return new Query(this)
            {
                queries = value
            };
        }

        public Query SetOptions(ImmutableList<IOption> value)
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

        public Query SetParameters(ImmutableList<IParameter> value)
        {
            return SetParameters(new Query(this), value);
        }

        public Query SetDocumentation(IDocumentation value)
        {
            return SetDocumentation(new Query(this), value);
        }

        public Builder ToBuilder()
        {
            return new Builder(this);
        }
    }
}
