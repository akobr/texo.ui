namespace BeaverSoft.Texo.Core.Configuration
{
    public partial class Option
    {
        public static Option Empty { get; } = new Option();

        public static Builder CreateBuilder()
        {
            return Empty.ToBuilder();
        }
    }
}
