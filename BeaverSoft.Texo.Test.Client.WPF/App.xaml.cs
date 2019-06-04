using System.Windows;
using BeaverSoft.Texo.Core;
using BeaverSoft.Texo.Core.Actions;
using BeaverSoft.Texo.Core.Actions.Implementations;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Runtime;
using BeaverSoft.Texo.Core.View;
using BeaverSoft.Texo.Core.View.Actions;
using BeaverSoft.Texo.Test.Client.WPF.Actions;
using BeaverSoft.Texo.Test.Client.WPF.Startup;
using StrongBeaver.Core.Container;
using StrongBeaver.Core.Services;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Test.Client.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static TexoEngine TexoEngine { get; private set; }

        public static IServiceMessageBus ServiceMessageBus { get; private set; }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            SimpleIoc container = new SimpleIoc();
            container.RegisterServices();

            TexoEngineBuilder engineBuilder =
                new TexoEngineBuilder()
                .WithLogService(new DebugLogService());

            container.RegisterEngineServices(engineBuilder);

            CommandFactory commandFactory = new CommandFactory();
            commandFactory.RegisterCommands(container);
            container.RegisterCommandFactory(commandFactory);

            engineBuilder.WithFallbackService(container.GetInstance<IFallbackService>());
            TexoEngine = engineBuilder.Build(commandFactory, container.GetInstance<IViewService>());
            TexoEngine.RegisterAction(new SimpleActionFactory<UriOpenAction>(), ActionNames.URI);
            TexoEngine.RegisterAction(new PathOpenActionFactory(container.GetInstance<IExecutor>()), ActionNames.PATH_OPEN, ActionNames.PATH);
            TexoEngine.RegisterAction(new InputSetActionFactory(container.GetInstance<IViewService>()), ActionNames.INPUT_SET, ActionNames.INPUT);

            ServiceMessageBus = container.GetInstance<IServiceMessageBus>();
            container.RegisterWithMessageBus();
            container.RegisterIntellisense();

            await TexoEngine.InitialiseWithCommandsAsync();        
            TexoEngine.Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            TexoEngine?.Dispose();
            TexoEngine = null;

            base.OnExit(e);
        }
    }
}
