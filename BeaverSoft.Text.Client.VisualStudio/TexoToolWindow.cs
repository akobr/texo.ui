using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using BeaverSoft.Texo.View.WPF;
using Microsoft;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell;

using Task = System.Threading.Tasks.Task;

namespace BeaverSoft.Text.Client.VisualStudio
{
    [Guid(WindowGuidString)]
    public class TexoToolWindow : ToolWindowPane
    {
        public const string WindowGuidString = "ca179975-59a3-4af6-b516-47017165b291";
        public const string MenuCommandGuidString = "6f375fee-28b1-4c82-9b0e-d7f973c68612";
        public const string Title = "Texo terminal";

        private readonly TexoToolWindowState state;

        // "state" parameter is the object returned from MyPackage.InitializeToolWindowAsync
        public TexoToolWindow(TexoToolWindowState state)
            : base()
        {
            this.state = state;
            Caption = Title;
            BitmapImageMoniker = KnownMonikers.ImageIcon;

            var wrapper = new TexoControlWrapper();
            TexoControl = wrapper.Texo;
            Content = wrapper;

            InitilialiseTexoControl();
        }

        public TexoControl TexoControl { get; }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            var commandService = (IMenuCommandService)await package.GetServiceAsync(typeof(IMenuCommandService));
            Assumes.Present(commandService);

            var cmdId = new CommandID(Guid.Parse(MenuCommandGuidString), 0x0100);
            var cmd = new MenuCommand((s, e) => Execute(package), cmdId);
            
            commandService.AddCommand(cmd);
        }

        private static void Execute(AsyncPackage package)
        {
            package.JoinableTaskFactory.RunAsync(async () =>
            {
                ToolWindowPane window = await package.ShowToolWindowAsync(
                    typeof(TexoToolWindow),
                    0,
                    create: true,
                    cancellationToken: package.DisposalToken);
            });
        }

        private void InitilialiseTexoControl()
        {
            WpfViewService wpfView = state.View;
            wpfView.Initialise(TexoControl);
        }

        private void InitilialiseClipboardControl()
        {
            //clipboardMonitor = new ClipboardMonitor(state.ServiceMessageBus);
            //FormsHost.Child = clipboardMonitor;
            //clipboardMonitor.Initialise();
        }
    }
}
