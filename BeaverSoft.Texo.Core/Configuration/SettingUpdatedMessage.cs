using StrongBeaver.Core.Services;

namespace BeaverSoft.Texo.Core.Configuration
{
    public class SettingUpdatedMessage : ServiceMessage, ISettingUpdatedMessage
    {
        public SettingUpdatedMessage(TexoConfiguration configuration)
        {
            Configuration = configuration;
        }

        public TexoConfiguration Configuration { get; }
    }
}
