namespace BeaverSoft.Texo.Core.Configuration
{
    public partial class Parameter
    {
        public static Parameter Empty { get; } = new Parameter();

        public static Builder CreateBuilder()
        {
            return Empty.ToBuilder();
        }
    }
}
