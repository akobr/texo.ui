namespace BeaverSoft.Texo.Core.Configuration
{
    public partial class Parameter
    {
        private string key;
        private bool isOptional;
        private bool isRepeatable;
        private string argumentTemplate;
        private Documentation documentation;

        public Parameter(string key, bool isOptional, bool isRepeatable, string argumentTemplate, Documentation documentation)
        {
            this.key = key;
            this.isOptional = isOptional;
            this.isRepeatable = isRepeatable;
            this.argumentTemplate = argumentTemplate;
            this.documentation = documentation;
        }

        private Parameter()
        {
            documentation = Documentation.Empty;
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
            documentation = builder.Documentation.ToImmutable();
        }

        public string Key => key;

        public bool IsOptional => isOptional;

        public bool IsRepeatable => isRepeatable;

        public string ArgumentTemplate => argumentTemplate;

        public Documentation Documentation => documentation;

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

        public Parameter SetDocumentation(Documentation value)
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
