using System.Windows;
using BeaverSoft.Texo.View.WPF;
using Commands.Clipboard;

namespace BeaverSoft.Texo.Test.Client.WPF
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Window
    {
        private ClipboardMonitor clipboardMonitor;

        public MainPage()
        {
            InitializeComponent();
            InitilialiseTexoControl();
            InitilialiseClipboardControl();
        }

        private void InitilialiseTexoControl()
        {
            WpfViewService wpfView = (WpfViewService)App.TexoEngine.View;
            wpfView.Initialise(TexoControl);
        }

        private void InitilialiseClipboardControl()
        {
            clipboardMonitor = new ClipboardMonitor(App.ServiceMessageBus);
            FormsHost.Child = clipboardMonitor;
            clipboardMonitor.Initialise();
        }
    }
}
