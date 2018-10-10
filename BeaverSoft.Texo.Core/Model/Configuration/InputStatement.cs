using System.Collections.Immutable;

namespace BeaverSoft.Texo.Core.Model.Configuration
{
    public abstract partial class InputStatement : IInputStatement
    {
        private string key;
        private ImmutableList<string> representations;
        private ImmutableList<IParameter> parameters;
        private IDocumentation documentation;

        protected InputStatement()
        {
            representations = ImmutableList<string>.Empty;
            parameters = ImmutableList<IParameter>.Empty;
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
            documentation = builder.Documentation;
        }

        protected InputStatement(
            string key,
            ImmutableList<string> representations,
            ImmutableList<IParameter> parameters,
            IDocumentation documentation)
        {
            this.key = key;
            this.representations = representations;
            this.parameters = parameters;
            this.documentation = documentation;
        }

        public string Key => key;

        public IImmutableList<string> Representations => representations;

        public IImmutableList<IParameter> Parameters => parameters;

        public IDocumentation Documentation => documentation;

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

        protected T SetParameters<T>(T cloned, ImmutableList<IParameter> value)
            where T : InputStatement
        {
            cloned.parameters = value;
            return cloned;
        }

        protected T SetDocumentation<T>(T cloned, IDocumentation value)
            where T : InputStatement
        {
            cloned.documentation = value;
            return cloned;
        }
    }
}
