using BeaverSoft.Texo.Core.Model.Configuration;

namespace BeaverSoft.Texo.Core.Configuration
{
    public interface ISettingsService
    {
        ITextumConfiguration Configuration { get; }

        void Configure(ITextumConfiguration configuration);
    }
}
