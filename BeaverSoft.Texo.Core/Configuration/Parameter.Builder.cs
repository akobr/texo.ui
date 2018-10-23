namespace BeaverSoft.Texo.Core.Configuration
{
    public partial class Parameter
    {
        public class Builder
        {
            internal Builder(Parameter parameter)
            {
                IsOptional = parameter.isOptional;
                IsRepeatable = parameter.isRepeatable;
                ArgumentTemplate = parameter.argumentTemplate;
                Documentation = parameter.documentation.ToBuilder();
            }

            public string Key { get; set; }

            public bool IsOptional { get; set; }

            public bool IsRepeatable { get; set; }

            public string ArgumentTemplate { get; set; }

            public Documentation.Builder Documentation { get; set; }

            public Parameter ToImmutable()
            {
                return new Parameter(this);
            }
        }
    }
}
