using System;
using BeaverSoft.Texo.Core;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Model.Configuration;
using BeaverSoft.Texo.Core.Services;
using BeaverSoft.Texo.Core.View;
using BeaverSoft.Texo.View.Console;
using BeaverSoft.Texo.View.Console.Markdown;
using GalaSoft.MvvmLight.Ioc;

namespace BeaverSoft.Texo.Test.Client.Console
{
    public class Program
    {
        private const string LONG_RUNNING_ARG = "run";

        private static TexoEngine engine;

        public static void Main(string[] args)
        {
            Startup();

            if (IsLongRunning(args))
            {
                LongRun();
            }
            else
            {
                Run(args);
            }

            Shutdown();
        }

        private static void Startup()
        {
            SimpleIoc container = new SimpleIoc();

            container.Register<ISettingService, SettingService>();
            container.Register<IMarkdownService, MarkdownService>();
            container.Register<IConsoleRenderService, ConsoleMarkdownRenderService>();
            container.Register<IViewService, ConsoleViewService>();

            engine = new TexoEngineBuilder()
                .WithSettingService(container.GetInstance<ISettingService>())
                .Build(container.GetInstance<IViewService>());

            engine.Configure(TextumConfiguration.CreateDefault());
        }

        private static void Shutdown()
        {
            engine?.Dispose();
        }

        private static void LongRun()
        {
            engine.Start();
        }

        private static void Run(string[] args)
        {
            engine.Process(string.Join(' ', args));
        }

        private static bool IsLongRunning(string[] args)
        {
            return args == null
                   || args.Length < 1
                   || string.Equals(args[0], LONG_RUNNING_ARG, StringComparison.OrdinalIgnoreCase);
        }
    }
}
