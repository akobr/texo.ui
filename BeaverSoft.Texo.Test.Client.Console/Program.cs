using System;
using System.Threading.Tasks;
using BeaverSoft.Texo.Commands.FileManager;
using BeaverSoft.Texo.Commands.FileManager.Stage;
using BeaverSoft.Texo.Commands.FileManager.Stash;
using BeaverSoft.Texo.Commands.NugetManager;
using BeaverSoft.Texo.Commands.NugetManager.Services;
using BeaverSoft.Texo.Core;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Help;
using BeaverSoft.Texo.Core.Input;
using BeaverSoft.Texo.Core.Runtime;
using BeaverSoft.Texo.Core.Services;
using BeaverSoft.Texo.Core.View;
using BeaverSoft.Texo.Fallback.PowerShell;
using BeaverSoft.Texo.Fallback.PowerShell.Markdown;
using BeaverSoft.Texo.View.Console;
using BeaverSoft.Texo.View.Console.Markdown;
using Commands.CommandLine;
using Commands.ReferenceCheck;
using StrongBeaver.Core.Container;
using StrongBeaver.Core.Services;
using StrongBeaver.Core.Services.Logging;
using StrongBeaver.Core.Services.Serialisation;
using StrongBeaver.Core.Services.Serialisation.Json;

namespace BeaverSoft.Texo.Test.Client.Console
{
    public class Program
    {
        private const string LONG_RUNNING_ARG = "run";

        private static TexoEngine engine;

        public async static Task Main(string[] args)
        {
            Startup();

            if (IsLongRunning(args))
            {
                LongRun();
            }
            else
            {
                await RunAsync(args);
            }

            Shutdown();
        }

        private static void Startup()
        {
            SimpleIoc container = new SimpleIoc();

            container.Register<ServiceMessageBus>();
            container.Register<IServiceMessageBus>(() => container.GetInstance<ServiceMessageBus>());
            container.Register<IServiceMessageBusRegister>(() => container.GetInstance<ServiceMessageBus>());

            container.Register<ILogService, DebugLogService>();
            container.Register<IInputParseService, InputParseService>();
            container.Register<IInputEvaluationService, InputEvaluationService>();
            container.Register<IEnvironmentService, EnvironmentService>();
            container.Register<ISettingService, SettingService>();
            container.Register<ICommandManagementService, SingletonCommandManagementService>();
            container.Register<IResultProcessingService, ResultProcessingService>();
            container.Register<IMarkdownService, MarkdownService>();
            container.Register<IConsoleRenderService, ConsoleMarkdownRenderService>();
            container.Register<ConsoleViewService>();
            container.Register<IViewService>(() => container.GetInstance<ConsoleViewService>());
            container.Register<IPromptableViewService>(() => container.GetInstance<ConsoleViewService>());

            // PowerShell Fallback
            container.Register<IPowerShellResultBuilder, PowerShellResultMarkdownBuilder>();
            container.Register<IFallbackService, PowerShellFallbackService>();

            // Core commands
            container.Register<CurrentDirectoryCommand>();
            container.Register<TexoCommand>();
            container.Register<HelpCommand>();
            container.Register<ClearCommand>();

            // Simple commands
            container.Register<ReferenceCheckCommand>();
            container.Register<CommandLineCommand>();

            // File manager
            container.Register<ISerialisationService, JsonSerialisationService>();
            container.Register<IStageService, StageService>();
            container.Register<IStashService, StashService>();
            container.Register<FileManagerCommand>();

            // Nuget manager
            container.Register<IProjectManagementService, ProjectManagementService>();
            container.Register<IPackageManagementService, PackageManagementService>();
            container.Register<IConfigManagementService, ConfigManagementService>();
            container.Register<ISourceManagementService, SourceManagementService>();
            container.Register<IManagementService, ManagementService>();
            container.Register<Commands.NugetManager.Stage.IStageService, Commands.NugetManager.Stage.StageService>();
            container.Register<NugetManagerCommand>();

            CommandFactory commandFactory = new CommandFactory();
            container.Register<ITexoFactory<ICommand, string>>(() => commandFactory);
            commandFactory.Register(CommandKeys.CURRENT_DIRECTORY, () => container.GetInstance<CurrentDirectoryCommand>());
            commandFactory.Register(CommandKeys.TEXO, () => container.GetInstance<TexoCommand>());
            commandFactory.Register(CommandKeys.HELP, container.GetInstance<HelpCommand>);
            commandFactory.Register(CommandKeys.CLEAR, container.GetInstance<ClearCommand>);
            commandFactory.Register(ReferenceCheckConstants.REF_CHECK, () => container.GetInstance<ReferenceCheckCommand>());
            commandFactory.Register("command-line", () => container.GetInstance<CommandLineCommand>());
            commandFactory.Register("file-manager", () => container.GetInstance<FileManagerCommand>());
            commandFactory.Register("nuget-manager", () => container.GetInstance<NugetManagerCommand>());

            ServiceMessageBus messageBus = container.GetInstance<ServiceMessageBus>();

            engine = new TexoEngineBuilder(messageBus, messageBus)
                .WithLogService(container.GetInstance<ILogService>())
                .WithInputParseService(container.GetInstance<IInputParseService>())
                .WithInputEvaluationService(container.GetInstance<IInputEvaluationService>())
                .WithEnvironmentService(container.GetInstance<IEnvironmentService>())
                .WithSettingService(container.GetInstance<ISettingService>())
                .WithCommandManagementService(container.GetInstance<ICommandManagementService>())
                .WithResultProcessingService(container.GetInstance<IResultProcessingService>())
                .WithFallbackService(container.GetInstance<IFallbackService>())
                .Build(commandFactory, container.GetInstance<IViewService>());

            var config = TextumConfiguration.CreateDefault().ToBuilder();
            config.Runtime.Commands.Add(ReferenceCheckCommand.BuildConfiguration());
            config.Runtime.Commands.Add(CommandLineCommand.BuildConfiguration());
            config.Runtime.Commands.Add(FileManagerBuilder.BuildCommand());
            config.Runtime.Commands.Add(NugetManagerBuilder.BuildCommand());

            engine.Initialise(config.ToImmutable());
        }

        private static void Shutdown()
        {
            engine?.Dispose();
        }

        private static void LongRun()
        {
            engine.Start();
        }

        private static Task RunAsync(string[] args)
        {
            return engine.ProcessAsync(string.Join(' ', args));
        }

        private static bool IsLongRunning(string[] args)
        {
            return args == null
                   || args.Length < 1
                   || string.Equals(args[0], LONG_RUNNING_ARG, StringComparison.OrdinalIgnoreCase);
        }
    }
}
