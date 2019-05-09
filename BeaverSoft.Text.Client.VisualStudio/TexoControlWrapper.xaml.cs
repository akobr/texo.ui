using System.Windows.Controls;
using BeaverSoft.Texo.View.WPF;

namespace BeaverSoft.Text.Client.VisualStudio
{
    /// <summary>
    /// Interaction logic for TexoControlWrapper.xaml
    /// </summary>
    public partial class TexoControlWrapper : UserControl
    {
        public TexoControlWrapper()
        {
            InitializeComponent();
        }

        public TexoControl Texo => texoControl;
    }
}
