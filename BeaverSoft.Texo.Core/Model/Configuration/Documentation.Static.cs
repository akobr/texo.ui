namespace BeaverSoft.Texo.Core.Model.Configuration
{
    public partial class Documentation
    {
        public static Documentation Empty { get; } = new Documentation();

        public static Builder CreateBuilder()
        {
            return Empty.ToBuilder();
        }
    }
}
