using StrongBeaver.Core.Services;

namespace BeaverSoft.Texo.Core.Configuration
{
    public class SettingService : ISettingService
    {
        private readonly IServiceMessageBus messageBus;

        public SettingService(IServiceMessageBus messageBus)
        {
            this.messageBus = messageBus;
        }

        public TexoConfiguration Configuration { get; private set; }

        public void Configure(TexoConfiguration configuration)
        {
            Configuration = configuration;
            messageBus?.Send<ISettingUpdatedMessage>(new SettingUpdatedMessage(configuration));
        }
    }
}
