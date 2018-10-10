namespace BeaverSoft.Texo.Core.Model.Configuration
{
    public partial class Documentation
    {
        public sealed class Builder
        {
            internal Builder(Documentation documentation)
            {
                Title = documentation.title;
                Description = documentation.description;
            }

            public string Title { get; set; }

            public string Description { get; set; }

            public Documentation ToImmutable()
            {
                return new Documentation(this);
            }
        }
    }
}
