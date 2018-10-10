namespace BeaverSoft.Texo.Core.Model.Configuration
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
