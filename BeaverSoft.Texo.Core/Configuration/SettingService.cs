using BeaverSoft.Texo.Core.Model.Configuration;

namespace BeaverSoft.Texo.Core.Configuration
{
    public class SettingService : ISettingService
    {
        public ITextumConfiguration Configuration { get; private set; }

        public void Configure(ITextumConfiguration configuration)
        {
            Configuration = configuration;
        }
    }
}
