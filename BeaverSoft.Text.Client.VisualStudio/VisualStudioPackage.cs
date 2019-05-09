using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core;
using BeaverSoft.Texo.Core.Actions;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Runtime;
using BeaverSoft.Texo.Core.View;
using BeaverSoft.Texo.View.WPF;
using BeaverSoft.Text.Client.VisualStudio.Actions;
using BeaverSoft.Text.Client.VisualStudio.Startup;
using Microsoft.VisualStudio.ProjectSystem;
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
        /// <summary>
        /// BeaverSoft.Text.Client.VisualStudioPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "2dc0bff1-fbaf-4c05-98a5-b1a2afc000cb";

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

            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await TexoToolWindow.InitializeAsync(this);
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
            var dteTask = GetServiceAsync(typeof(EnvDTE.DTE));
            var threadingTask = GetServiceAsync(typeof(IProjectThreadingService));

            var container = new SimpleIoc();
            container.RegisterServices();

            var engineBuilder = new TexoEngineBuilder()
                .WithLogService(new DebugLogService())
                .WithFallbackService(container.GetInstance<IFallbackService>());

            container.RegisterEngineServices(engineBuilder);

            var commandFactory = new CommandFactory();
            commandFactory.RegisterCommands(container);
            container.RegisterCommandFactory(commandFactory);

            var texoEngine = engineBuilder.Build(commandFactory, container.GetInstance<IViewService>());

            var messageBus = container.GetInstance<IServiceMessageBus>();
            container.RegisterWithMessageBus();
            container.RegisterIntellisense();

            var context = new ExtensionContext(
                (EnvDTE80.DTE2)await dteTask,
                (IProjectThreadingService)await threadingTask,
                texoEngine,
                messageBus);

            // Register of actions
            texoEngine.RegisterAction(new PathOpenActionFactory(context), ActionNames.PATH_OPEN, ActionNames.PATH);

            await texoEngine.InitialiseWithCommandsAsync();
            texoEngine.Start();
            return context;
        }

        #endregion
    }
}
