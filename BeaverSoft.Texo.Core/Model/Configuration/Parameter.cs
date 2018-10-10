namespace BeaverSoft.Texo.Core.Model.Configuration
{
    public partial class Parameter : IParameter
    {
        private string key;
        private bool isOptional;
        private bool isRepeatable;
        private string argumentTemplate;
        private IDocumentation documentation;

        private Parameter()
        {
            // no operation
        }

        private Parameter(Parameter toClone)
        {
            key = toClone.key;
            isOptional = toClone.isOptional;
            isRepeatable = toClone.isRepeatable;
            argumentTemplate = toClone.argumentTemplate;
            documentation = toClone.documentation;
        }

        private Parameter(Builder builder)
        {
            key = builder.Key;
            isOptional = builder.IsOptional;
            isRepeatable = builder.IsRepeatable;
            argumentTemplate = builder.ArgumentTemplate;
            documentation = builder.Documentation;
        }

        public string Key => key;

        public bool IsOptional => isOptional;

        public bool IsRepeatable => isRepeatable;

        public string ArgumentTemplate => argumentTemplate;

        public IDocumentation Documentation => documentation;

        public Parameter SetKey(string value)
        {
            return new Parameter(this)
            {
                key = value
            };
        }

        public Parameter SetIsOptional(bool value)
        {
            return new Parameter(this)
            {
                isOptional = value
            };
        }

        public Parameter SetIsRepeatable(bool value)
        {
            return new Parameter(this)
            {
                isRepeatable = value
            };
        }

        public Parameter SetArgumentTemplate(string value)
        {
            return new Parameter(this)
            {
                argumentTemplate = value
            };
        }

        public Parameter SetDocumentation(IDocumentation value)
        {
            return new Parameter(this)
            {
                documentation = value
            };
        }

        public Builder ToBuilder()
        {
            return new Builder(this);
        }
    }
}
