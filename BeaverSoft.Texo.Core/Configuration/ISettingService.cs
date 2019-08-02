namespace BeaverSoft.Texo.Core.Configuration
{
    public interface ISettingService
    {
        TexoConfiguration Configuration { get; }

        void Configure(TexoConfiguration configuration);
    }
}
