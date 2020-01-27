using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
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
            InitialiseTheme();
            InitilialiseTexoControl();
            InitilialiseClipboardControl();

            //ConsoleManager.Show();

            //Console.WriteLine("Application running!");
            //Console.ForegroundColor = ConsoleColor.Green;
            //Console.WriteLine("colors");
            //Console.ResetColor();
        }

        private void InitialiseTheme()
        {
            BorderBrush = new SolidColorBrush(SystemParameters.WindowGlassColor);
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

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) { DragMove(); }
        }

        private void MaximizeRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
                MaximizeRestoreButton.Content = "\uE923";
            }
            else if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                MaximizeRestoreButton.Content = "\uE922";
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
