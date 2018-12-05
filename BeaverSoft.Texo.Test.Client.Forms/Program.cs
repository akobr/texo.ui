using System;
using System.Windows.Forms;
using BeaverSoft.Texo.Core;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Runtime;
using BeaverSoft.Texo.Core.View;
using BeaverSoft.Texo.Test.Client.Forms.Startup;
using StrongBeaver.Core.Container;

namespace BeaverSoft.Texo.Test.Client.Forms
{
    static class Program
    {
        public static TexoEngine TexoEngine { get; private set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            StartUp();
            Application.ApplicationExit += OnApplicationExit;
            Application.Run(new MainForm());
        }

        private static void OnApplicationExit(object sender, EventArgs e)
        {
            TexoEngine?.Dispose();
            TexoEngine = null;
        }

        private static void StartUp()
        {
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
    }
}
