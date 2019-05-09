using System.Windows;
using System.Windows.Controls;

namespace BeaverSoft.Text.Client.VisualStudio
{
    // https://github.com/microsoft/VSSDK-Extensibility-Samples/blob/master/AsyncToolWindow/src/ToolWindows/SampleToolWindowControl.xaml.cs

    /// <summary>
    /// Interaction logic for EmptyControl.xaml
    /// </summary>
    public partial class EmptyControl : UserControl
    {
        private readonly ExtensionContext state;

        public EmptyControl(ExtensionContext state)
        {
            this.state = state;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string version = state.DTE.FullName;
            MessageBox.Show($"Visual Studio is located here: '{version}'");
        }
    }
}
