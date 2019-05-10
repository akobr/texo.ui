using System;
using System.Windows;
using BeaverSoft.Texo.Fallback.PowerShell;
using BeaverSoft.Texo.View.WPF;
using Commands.Clipboard;

namespace BeaverSoft.Texo.Test.Client.WPF
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Window
    {
        private ClipboardMonitorControl clipboardMonitor;

        public MainPage()
        {
            InitializeComponent();
            InitilialiseTexoControl();
            InitilialiseClipboardControl();

            //ConsoleManager.Show();

            //Console.WriteLine("Application running!");
            //Console.ForegroundColor = ConsoleColor.Green;
            //Console.WriteLine("colors");
            //Console.ResetColor();
        }

        private void InitilialiseTexoControl()
        {
            WpfViewService wpfView = (WpfViewService)App.TexoEngine.View;
            wpfView.Initialise(TexoControl);
        }

        private void InitilialiseClipboardControl()
        {
            clipboardMonitor = new ClipboardMonitorControl(App.ServiceMessageBus);
            FormsHost.Child = clipboardMonitor;
            clipboardMonitor.Initialise();
        }
    }
}
