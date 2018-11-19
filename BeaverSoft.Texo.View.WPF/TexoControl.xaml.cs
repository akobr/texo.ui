using System;
using System.Windows;
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
        public event EventHandler<HistoryScrollDirection> CommandHistoryScrolled;

        public RichTextBox InputBox => tbInput;

        public FlowDocumentScrollViewer OutputDocument => docOutput;

        public string Prompt
        {
            get => lbPrompt.Text;
            set => lbPrompt.Text = value;
        }

        public string Title
        {
            get => lbTitle.Text;
            set => lbTitle.Text = value;
        }

        public void SetHistoryCount(int count)
        {
            lbHistoryCount.Text = $"History ({count})";
        }

        public void SetVariableCount(int count)
        {
            lbVariableCount.Text = $"Variables ({count})";
        }

        private void HandleInputTextChanged(object sender, TextChangedEventArgs e)
        {
            //InputChanged?.Invoke(this, GetInput());
        }

        private void HandleInputKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string input = GetInput();

                if (!string.IsNullOrWhiteSpace(input))
                {
                    InputFinished?.Invoke(this, GetInput());
                }

                e.Handled = true;
            }
        }

        private void HandleInputKeyDownPreview(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                CommandHistoryScrolled?.Invoke(this, HistoryScrollDirection.Back);
            }
            else if (e.Key == Key.Down)
            {
                CommandHistoryScrolled?.Invoke(this, HistoryScrollDirection.Forward);
            }
        }

        private void HandleInputSelectionChanged(object sender, RoutedEventArgs e)
        {
            TextRange range = new TextRange(tbInput.Document.ContentStart, tbInput.Document.ContentEnd);
            range.ApplyPropertyValue(RichTextBox.FontSizeProperty, 16.0);
        }

        private string GetInput()
        {
            TextRange range = new TextRange(tbInput.Document.ContentStart, tbInput.Document.ContentEnd);
            return range.Text.Trim();
        }
    }
}