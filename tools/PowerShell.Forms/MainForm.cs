using System.Windows.Forms;
using BeaverSoft.Texo.Fallback.PowerShell;

namespace PowerShell.Forms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            BuildService();
        }

        private void BuildService()
        {
            TexoPowerShellHost host = new TexoPowerShellHost(null, null, null);
        }
    }
}
