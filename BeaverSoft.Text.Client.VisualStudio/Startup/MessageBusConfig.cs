using BeaverSoft.Texo.Fallback.PowerShell;
using StrongBeaver.Core.Container;
using StrongBeaver.Core.Services;

namespace BeaverSoft.Text.Client.VisualStudio.Startup
{
    public static class MessageBusConfig
    {
        public static void RegisterWithMessageBus(this SimpleIoc container)
        {
            IServiceMessageBusRegister register = container.GetInstance<IServiceMessageBusRegister>();

            register.Register(container.GetInstance<PowerShellFallbackService>());
            //register.Register(container.GetInstance<ClipboardMonitoringService>());
        }
    }
}
