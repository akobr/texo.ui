using System.Windows;
using BeaverSoft.Texo.Core;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Runtime;
using BeaverSoft.Texo.Core.View;
using BeaverSoft.Texo.Test.Client.WPF.Startup;
using StrongBeaver.Core.Container;

namespace BeaverSoft.Texo.Test.Client.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static TexoEngine TexoEngine { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            SimpleIoc container = new SimpleIoc();
            container.RegisterServices();

            TexoEngineBuilder engineBuilder = new TexoEngineBuilder();
            container.RegisterEngineServices(engineBuilder);

            CommandFactory commandFactory = new CommandFactory();
            commandFactory.RegisterCommands(container);
            container.RegisterCommandFactory(commandFactory);

            engineBuilder.WithFallbackService(container.GetInstance<IFallbackService>());
            TexoEngine = engineBuilder.Build(commandFactory, container.GetInstance<IViewService>());
            TexoEngine.InitialiseWithCommands();
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
