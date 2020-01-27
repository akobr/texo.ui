using BeaverSoft.Texo.Fallback.PowerShell;
using BeaverSoft.Texo.Fallback.PseudoConsole;
using Commands.Clipboard;
using StrongBeaver.Core.Container;
using StrongBeaver.Core.Services;

namespace BeaverSoft.Texo.Test.Client.WPF.Startup
{
    public static class MessageBusConfig
    {
        public static void RegisterWithMessageBus(this SimpleIoc container)
        {
            IServiceMessageBusRegister register = container.GetInstance<IServiceMessageBusRegister>();

            //register.Register(container.GetInstance<PowerShellFallbackService>());
            register.Register(container.GetInstance<PseudoConsoleFallbackService>());
            register.Register(container.GetInstance<ClipboardMonitoringService>());
        }
    }
}
