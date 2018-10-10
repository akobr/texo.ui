namespace BeaverSoft.Texo.Core.Model.Configuration
{
    public partial class Option
    {
        public sealed class Builder : BaseBuilder
        {
            internal Builder(Option configuration)
                : base(configuration)
            {
                // no operation
            }

            public Option ToImmutable()
            {
                return new Option(this);
            }
        }
    }
}
