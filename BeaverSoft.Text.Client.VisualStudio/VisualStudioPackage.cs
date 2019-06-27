using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core;
using BeaverSoft.Texo.Core.Actions;
using BeaverSoft.Texo.Core.Actions.Implementations;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Runtime;
using BeaverSoft.Texo.Core.View;
using BeaverSoft.Texo.Core.View.Actions;
using BeaverSoft.Text.Client.VisualStudio.Actions;
using BeaverSoft.Text.Client.VisualStudio.Environment;
using BeaverSoft.Text.Client.VisualStudio.Search;
using BeaverSoft.Text.Client.VisualStudio.Startup;
using Commands.CodeBaseSearch;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using StrongBeaver.Core.Container;
using StrongBeaver.Core.Services;
using StrongBeaver.Core.Services.Logging;
using Task = System.Threading.Tasks.Task;

namespace BeaverSoft.Text.Client.VisualStudio
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [InstalledProductRegistration("Texo terminal", "Powerful terminal with built-in commands and PowerShell fallback.", "1.0")]
    [ProvideToolWindow(typeof(TexoToolWindow), Style = VsDockStyle.Tabbed, DockedWidth = 400, Window = "DocumentWell", Orientation = ToolWindowOrientation.Right)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(PackageGuidString)]
    public sealed class VisualStudioPackage : AsyncPackage
    {
        // Log from launch available at:
        // c:\Users\[USER_NAME]\AppData\Roaming\Microsoft\VisualStudio\16.0_c3776c56Exp\ActivityLog.xml

        public const string PackageGuidString = "2dc0bff1-fbaf-4c05-98a5-b1a2afc000cb";
        public static bool IsMarkdownAssemblyLoaded;
        public static bool IsNewtonsoftJsonAssemblyLoaded;

        private ICodeBaseSearchService codeSearch;
        private SolutionEvents solutionEvents;
        private _dispSolutionEvents_OpenedEventHandler solutionOpenedEventHandler;

        public EnvDTE80.DTE2 DTE { get; private set; }

        public IComponentModel ComponentModel { get; private set; }

        public ExtensionContext Context { get; private set; }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // Hack: force load of Markdig.Wpf assembly 
            Type markdownType = typeof(Markdig.Wpf.Markdown);
            IsMarkdownAssemblyLoaded = markdownType != null;
            Type jsonType = typeof(Newtonsoft.Json.JsonSerializer);
            IsNewtonsoftJsonAssemblyLoaded = jsonType != null;

            DTE = (EnvDTE80.DTE2)await GetServiceAsync(typeof(DTE));
            ComponentModel = (IComponentModel)await GetServiceAsync(typeof(SComponentModel));

            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            solutionEvents = DTE.Events.SolutionEvents;
            solutionOpenedEventHandler = new _dispSolutionEvents_OpenedEventHandler(HandleSolutionOpened);
            solutionEvents.Opened += solutionOpenedEventHandler;

            await TexoToolWindow.InitializeAsync(this);
        }

        private async void HandleSolutionOpened()
        {
            if (Context == null)
            {
                return;
            }

            TriggerCodeBaseSearchLoad();
            await JoinableTaskFactory.SwitchToMainThreadAsync();
            Context.TexoEnvironment.SetVariable(VsVariableNames.SOLUTION_DIRECTORY, Path.GetDirectoryName(DTE.Solution.FileName));
            Context.TexoEnvironment.SetVariable(VsVariableNames.SOLUTION_FILE, DTE.Solution.FileName);
        }

        private async void TriggerCodeBaseSearchLoad()
        {
            await Task.Delay(15000);
            _ = codeSearch.PreLoadAsync().ContinueWith(async (preLoadTask) =>
            {
                if (preLoadTask.IsFaulted)
                {
                    Context.Logger.Error("Preload of code-search failed.", preLoadTask.Exception);
                    return;
                }

                try
                {
                    await codeSearch.LoadAsync();
                }
                catch (Exception exception)
                {
                    Context.Logger.Error("Load of code-search failed.", exception);
                }
            }, TaskScheduler.Default);
        }

        public override IVsAsyncToolWindowFactory GetAsyncToolWindowFactory(Guid toolWindowType)
        {
            return toolWindowType.Equals(Guid.Parse(TexoToolWindow.WindowGuidString)) ? this : null;
        }

        protected override string GetToolWindowTitle(Type toolWindowType, int id)
        {
            return toolWindowType == typeof(TexoToolWindow) ? TexoToolWindow.Title : base.GetToolWindowTitle(toolWindowType, id);
        }

        protected override async Task<object> InitializeToolWindowAsync(Type toolWindowType, int id, CancellationToken cancellationToken)
        {
            // Perform as much work as possible in this method which is being run on a background thread.
            // The object returned from this method is passed into the constructor of the SampleToolWindow 
            var container = new SimpleIoc();
            container.RegisterServices();

            var engineBuilder = new TexoEngineBuilder()
                .WithLogService(new DebugLogService());

            container.RegisterEngineServices(engineBuilder);
            engineBuilder.WithFallbackService(container.GetInstance<IFallbackService>());

            var commandFactory = new CommandFactory();
            commandFactory.RegisterCommands(container);
            container.RegisterCommandFactory(commandFactory);

            var texoEngine = engineBuilder.Build(commandFactory, container.GetInstance<IViewService>());

            var messageBus = container.GetInstance<IServiceMessageBus>();
            container.RegisterWithMessageBus();
            container.RegisterIntellisense();

            var environment = container.GetInstance<IEnvironmentService>();
            container.Register<ISolutionOpenStrategy>(() => new CurrentSolutionOpenStrategy(ComponentModel));
            codeSearch = container.GetInstance<ICodeBaseSearchService>();

            Context = new ExtensionContext(
                DTE,
                JoinableTaskFactory,
                texoEngine,
                environment,
                messageBus);

            // Register of variable strategies
            environment.RegisterVariableStrategy(VsVariableNames.SOLUTION_DIRECTORY, new SolutionDirectoryStrategy(environment));

            // Register of actions
            texoEngine.RegisterAction(new SimpleActionFactory<UriOpenAction>(), ActionNames.URI);
            texoEngine.RegisterAction(new PathOpenActionFactory(Context), ActionNames.PATH_OPEN, ActionNames.PATH);
            texoEngine.RegisterAction(new InputSetActionFactory(container.GetInstance<IViewService>()), ActionNames.INPUT_SET, ActionNames.INPUT);

            await texoEngine.InitialiseWithCommandsAsync();
            texoEngine.Start();
            return Context;
        }

        #endregion
    }
}
