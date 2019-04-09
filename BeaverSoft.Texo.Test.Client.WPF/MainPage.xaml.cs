using System.Windows;
using System.Windows.Controls;
using BeaverSoft.Texo.View.WPF;

namespace BeaverSoft.Texo.Test.Client.WPF
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Window
    {
        public MainPage()
        {
            InitializeComponent();
            InitilialiseTexoControl();
        }

        private void InitilialiseTexoControl()
        {
            WpfViewService wpfView = (WpfViewService)App.TexoEngine.View;
            wpfView.Initialise(TexoControl);
        }
    }
}
