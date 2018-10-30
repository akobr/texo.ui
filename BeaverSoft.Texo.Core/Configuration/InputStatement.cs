using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Configuration
{
    public abstract partial class InputStatement
    {
        private string key;
        private ImmutableList<string> representations;
        private ImmutableList<Parameter> parameters;
        private Documentation documentation;

        protected InputStatement()
        {
            representations = ImmutableList<string>.Empty;
            parameters = ImmutableList<Parameter>.Empty;
            documentation = Documentation.Empty;
        }

        protected InputStatement(InputStatement toClone)
        {
            key = toClone.key;
            representations = toClone.representations;
            parameters = toClone.parameters;
            documentation = toClone.documentation;
        }

        protected InputStatement(BaseBuilder builder)
        {
            key = builder.Key;
            representations = builder.Representations.ToImmutable();
            parameters = builder.Parameters.ToImmutable();
            documentation = builder.Documentation.ToImmutable();
        }

        protected InputStatement(
            string key,
            ImmutableList<string> representations,
            ImmutableList<Parameter> parameters,
            Documentation documentation)
        {
            this.key = key;
            this.representations = representations;
            this.parameters = parameters;
            this.documentation = documentation;
        }

        public string Key => key;

        public ImmutableList<string> Representations => representations;

        public ImmutableList<Parameter> Parameters => parameters;

        public Documentation Documentation => documentation;

        protected T SetKey<T>(T cloned, string value)
            where T : InputStatement
        {
            cloned.key = value;
            return cloned;
        }

        protected T SetRepresentations<T>(T cloned, ImmutableList<string> value)
            where T : InputStatement
        {
            cloned.representations = value;
            return cloned;
        }

        protected T SetParameters<T>(T cloned, ImmutableList<Parameter> value)
            where T : InputStatement
        {
            cloned.parameters = value;
            return cloned;
        }

        protected T SetDocumentation<T>(T cloned, Documentation value)
            where T : InputStatement
        {
            cloned.documentation = value;
            return cloned;
        }
    }
}
