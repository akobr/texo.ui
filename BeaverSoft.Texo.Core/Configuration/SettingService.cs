using BeaverSoft.Texo.Core.Model.Configuration;

namespace BeaverSoft.Texo.Core.Configuration
{
    public class SettingService : ISettingService
    {
        public TextumConfiguration Configuration { get; private set; }

        public void Configure(TextumConfiguration configuration)
        {
            Configuration = configuration;
        }
    }
}
