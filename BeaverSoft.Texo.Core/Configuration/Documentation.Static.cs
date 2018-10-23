namespace BeaverSoft.Texo.Core.Configuration
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
