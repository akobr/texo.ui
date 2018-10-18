using BeaverSoft.Texo.Core.Model.Configuration;

namespace BeaverSoft.Texo.Core.Configuration
{
    public interface ISettingService
    {
        ITextumConfiguration Configuration { get; }

        void Configure(ITextumConfiguration configuration);
    }
}
