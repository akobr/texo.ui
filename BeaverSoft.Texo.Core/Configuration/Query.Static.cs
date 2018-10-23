namespace BeaverSoft.Texo.Core.Configuration
{
    public partial class Query
    {
        public static Query Empty { get; } = new Query();

        public static Builder CreateBuilder()
        {
            return Empty.ToBuilder();
        }
    }
}
