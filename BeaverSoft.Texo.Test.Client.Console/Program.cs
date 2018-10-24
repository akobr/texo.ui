using System;
using BeaverSoft.Texo.Core;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Input;
using BeaverSoft.Texo.Core.Runtime;
using BeaverSoft.Texo.Core.Services;
using BeaverSoft.Texo.Core.View;
using BeaverSoft.Texo.View.Console;
using BeaverSoft.Texo.View.Console.Markdown;
using Commands.ReferenceCheck;
using GalaSoft.MvvmLight.Ioc;
using StrongBeaver.Core.Services;
using StrongBeaver.Core.Services.Logging;

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

            container.Register<ServiceMessageBus>();
            container.Register<IServiceMessageBus>(() => container.GetInstance<ServiceMessageBus>());

            container.Register<ILogService, DebugLogService>();
            container.Register<IInputParseService, InputParseService>();
            container.Register<IInputEvaluationService, InputEvaluationService>();
            container.Register<IEnvironmentService, EnvironmentService>();
            container.Register<ISettingService, SettingService>();
            container.Register<ICommandManagementService, SingletonCommandManagementService>();
            container.Register<IResultProcessingService, ResultProcessingService>();
            container.Register<IMarkdownService, MarkdownService>();
            container.Register<IConsoleRenderService, ConsoleMarkdownRenderService>();
            container.Register<IViewService, ConsoleViewService>();

            container.Register<ICurrentDirectoryService, CurrentDirectoryService>();
            container.Register<CurrentDirectoryCommand>();
            container.Register<TexoCommand>();

            CommandFactory commandFactory = new CommandFactory();
            container.Register<ITexoFactory<ICommand, string>>(() => commandFactory);
            commandFactory.Register(CommandKeys.CURRENT_DIRECTORY, () => container.GetInstance<CurrentDirectoryCommand>());
            commandFactory.Register(CommandKeys.TEXO, () => container.GetInstance<TexoCommand>());

            engine = new TexoEngineBuilder(container.GetInstance<ServiceMessageBus>())
                .WithLogService(container.GetInstance<ILogService>())
                .WithInputParseService(container.GetInstance<IInputParseService>())
                .WithInputEvaluationService(container.GetInstance<IInputEvaluationService>())
                .WithEnvironmentService(container.GetInstance<IEnvironmentService>())
                .WithSettingService(container.GetInstance<ISettingService>())
                .WithCommandFactory(commandFactory)
                .WithCommandManagementService(container.GetInstance<ICommandManagementService>())
                .WithResultProcessingService(container.GetInstance<IResultProcessingService>())
                .Build(container.GetInstance<IViewService>());

            engine.Initialise();

            var config = TextumConfiguration.CreateDefault().ToBuilder();
            config.Runtime.Commands.Add(ReferenceCheckCommand.BuildConfiguration());
            engine.Configure(config.ToImmutable());
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
