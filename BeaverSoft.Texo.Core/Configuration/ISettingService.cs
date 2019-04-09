namespace BeaverSoft.Texo.Core.Configuration
{
    public interface ISettingService
    {
        TextumConfiguration Configuration { get; }

        void Configure(TextumConfiguration configuration);
    }
}
