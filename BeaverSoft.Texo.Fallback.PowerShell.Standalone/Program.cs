using System;
using System.Threading.Tasks;
using BeaverSoft.Texo.Fallback.PowerShell.Core;
using BeaverSoft.Texo.Fallback.PowerShell.Standalone.Services;
using BeaverSoft.Texo.Fallback.PowerShell.Standalone.Startup;
using StrongBeaver.Core.Container;

namespace BeaverSoft.Texo.Fallback.PowerShell.Standalone.Native
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var container = Startup();
            var shutdown = container.GetInstance<IShutdownService>();
            await container.GetInstance<ICommunicationService>().StartAsync(shutdown.CancellationToken);

            await Task.Delay(2000);

            IPowerShellService service = container.GetInstance<IPowerShellService>();
            service.AddCommandToQueue(new CommandRequest(Guid.NewGuid(), "git status"));

            shutdown.Wait();
        }

        static SimpleIoc Startup()
        {
            SimpleIoc container = new SimpleIoc();

            container.ConfigureLogging();
            container.RegisterServices();

            return container;
        }
    }
}
