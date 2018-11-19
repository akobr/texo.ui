using System.Windows.Controls;
using BeaverSoft.Texo.View.WPF;

namespace BeaverSoft.Texo.Test.Client.WPF
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
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
