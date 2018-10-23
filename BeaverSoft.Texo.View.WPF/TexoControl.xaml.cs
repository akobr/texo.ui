using System;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace BeaverSoft.Texo.View.WPF
{
    /// <summary>
    /// Interaction logic for TexoControl.xaml
    /// </summary>
    public partial class TexoControl : UserControl
    {
        public TexoControl()
        {
            InitializeComponent();
        }

        public event EventHandler<string> InputChanged;
        public event EventHandler<string> InputFinished;

        public RichTextBox InputBox => tbInput;

        public FlowDocumentScrollViewer OutputDocument => docOutput;

        private void HandleInputTextChanged(object sender, TextChangedEventArgs e)
        {
            InputChanged?.Invoke(this, GetInput());
        }

        private void HandleInputKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                InputFinished?.Invoke(this, GetInput());
            }
        }

        private string GetInput()
        {
            TextRange range = new TextRange(tbInput.Document.ContentStart, tbInput.Document.ContentEnd);
            return range.Text;
        }
    }
}
