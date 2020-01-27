using System;
using System.Threading;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Inputting;
using BeaverSoft.Texo.Core.View;
using BeaverSoft.Texo.Fallback.PowerShell.Standalone.Communication;
using BeaverSoft.Texo.Fallback.PowerShell.Standalone.NamedPipes;
using BeaverSoft.Texo.Fallback.PowerShell.Standalone.Services;
using StrongBeaver.Core.Container;
using StrongBeaver.Core.Services;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Fallback.PowerShell.Standalone.Startup
{
    public static class ContainerConfig
    {
        public static void RegisterServices(this SimpleIoc container)
        {
            container.Register<IServiceMessageBus, EmptyServiceMessageBus>();
            container.Register<IEnvironmentService, EmptyEnvironmentService>();
            container.Register<IPromptableViewService, RemotePromptableViewService>();
            container.Register<IInputParseService, InputParseService>();
            container.Register<IShutdownService, ShutdownService>();
            container.Register<IPowerShellService, PowerShellService>();
            container.Register<ICommunicationService, CommunicationService>();

            container.Register<InputEvaluationService>();
            container.Register<IInputEvaluationService>(() =>
            {
                InputEvaluationService evaluationService = container.GetInstance<InputEvaluationService>();
                evaluationService.PrepareInputTree(TexoConfiguration.CreateDefault());
                return evaluationService;
            });

            container.Register<ICommunicator>(() =>
            {
                return new Communicator(new NamedPipeClient(container.GetInstance<ILogService>()));
            });
        }
    }
}
