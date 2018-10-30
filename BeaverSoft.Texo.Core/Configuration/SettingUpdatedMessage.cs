using StrongBeaver.Core.Services;

namespace BeaverSoft.Texo.Core.Configuration
{
    public class SettingUpdatedMessage : ServiceMessage, ISettingUpdatedMessage
    {
        public SettingUpdatedMessage(TextumConfiguration configuration)
        {
            Configuration = configuration;
        }

        public TextumConfiguration Configuration { get; }
    }
}
