using BeaverSoft.Texo.Commands.FileManager;
using BeaverSoft.Texo.Commands.FileManager.Stage;
using BeaverSoft.Texo.Commands.FileManager.Stash;
using BeaverSoft.Texo.Commands.NugetManager;
using BeaverSoft.Texo.Commands.NugetManager.Services;
using BeaverSoft.Texo.Core;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Help;
using BeaverSoft.Texo.Core.Input.History;
using BeaverSoft.Texo.Core.Runtime;
using BeaverSoft.Texo.Core.Services;
using BeaverSoft.Texo.Core.View;
using BeaverSoft.Texo.Fallback.PowerShell;
using BeaverSoft.Texo.Fallback.PowerShell.Markdown;
using BeaverSoft.Texo.View.WPF;
using BeaverSoft.Texo.View.WPF.Markdown;
using Commands.CommandLine;
using Commands.ReferenceCheck;
using StrongBeaver.Core;
using StrongBeaver.Core.Container;
using StrongBeaver.Core.Services.Serialisation;
using StrongBeaver.Core.Services.Serialisation.Json;

namespace BeaverSoft.Texo.Test.Client.WPF.Startup
{
    public static class ContainerConfig
    {
        public static void RegisterServices(this SimpleIoc container)
        {
            // Core commands (should be in engine)
            container.Register<ICurrentDirectoryService, CurrentDirectoryService>();
            container.Register<CurrentDirectoryCommand>();
            container.Register<TexoCommand>();
            container.Register<HelpCommand>();
            container.Register<ClearCommand>();

            // Simple commands
            container.Register<ReferenceCheckCommand>();
            //container.Register<DirCommand>();
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

            // View
            container.Register<IMarkdownService, MarkdownService>();
            container.Register<IWpfRenderService, WpfMarkdownRenderService>();
            container.Register<WpfViewService>();
            container.Register<IViewService>(container.GetInstance<WpfViewService>);
            container.Register<IPromptableViewService>(container.GetInstance<WpfViewService>);

            // PowerShell Fallback
            container.Register<IPowerShellResultBuilder, PowerShellResultMarkdownBuilder>();
            container.Register<IFallbackService, PowerShellFallbackService>();
        }

        public static void RegisterEngineServices(this SimpleIoc container, TexoEngineBuilder builder)
        {
            ITexoEngineServiceLocator engineServiceLocator = builder.GetServiceLocator();

            container.Register(engineServiceLocator.MessageBus);
            container.Register(engineServiceLocator.MessageBusRegister);
            container.Register(engineServiceLocator.Logger);
            container.Register(engineServiceLocator.History);
            container.Register(engineServiceLocator.Environment);
            container.Register(engineServiceLocator.Setting);
            container.Register<IFactory<IInputHistoryService>>(() => new DelegatedFactory<IInputHistoryService>(engineServiceLocator.History));
        }

        public static void RegisterCommandFactory(this SimpleIoc container, CommandFactory factory)
        {
            container.Register<CommandFactory>(() => factory);
            container.Register<ITexoFactory<ICommand, string>>(() => factory);
        }
    }
}
