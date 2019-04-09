namespace BeaverSoft.Texo.Core.Configuration
{
    public partial class Documentation
    {
        private string title;
        private string description;

        public Documentation(string title, string description)
        {
            this.title = title;
            this.description = description;
        }

        private Documentation()
        {
            // no operation
        }

        private Documentation(Documentation toClone)
        {
            title = toClone.title;
            description = toClone.description;
        }

        private Documentation(Builder builder)
        {
            title = builder.Title;
            description = builder.Description;
        }

        public string Title => title;

        public string Description => description;

        protected Documentation SetTitle(string value)
        {
            return new Documentation(this)
            {
                title = value
            };
        }

        protected Documentation SetDescription(string value)
        {
            return new Documentation(this)
            {
                description = value
            };
        }

        public Builder ToBuilder()
        {
            return new Builder(this);
        }
    }
}
