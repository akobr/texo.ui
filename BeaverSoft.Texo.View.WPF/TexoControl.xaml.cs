using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

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
            docOutput.Document = new FlowDocument();
        }

        public event EventHandler<string> InputChanged;
        public event EventHandler<string> InputFinished;
        public event EventHandler<HistoryScrollDirection> CommandHistoryScrolled;

        public FlowDocument OutputDocument => docOutput.Document;

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

        public void EnableInput()
        {
            tbInput.IsReadOnly = false;
        }

        public void DisableInput()
        {
            tbInput.IsReadOnly = true;
        }

        public void EmptyInput()
        {
            tbInput.Text = string.Empty;

            //TextRange range = new TextRange(
            //    tbInput.Document.ContentStart,
            //    tbInput.Document.ContentEnd);
            //range.Text = string.Empty;
        }

        public string GetInput()
        {
            return tbInput.Text;

            //TextRange range = new TextRange(tbInput.Document.ContentStart, tbInput.Document.ContentEnd);
            //return range.Text.Trim();
        }

        public void SetInput(string input)
        {
            tbInput.Text = input;
            tbInput.CaretIndex = tbInput.Text.Length;

            //TextRange range = new TextRange(control.InputBox.Document.ContentStart, control.InputBox.Document.ContentEnd);
            //range.Text = historyItem.Input.ParsedInput.RawInput;
            //control.InputBox.CaretPosition = control.InputBox.Document.ContentEnd;
        }

        public void FocusInput()
        {
            tbInput.Focus();
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
            InputChanged?.Invoke(this, GetInput());
        }

        private void HandleInputKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                return;
            }

            e.Handled = true;
            string input = GetInput();

            if (string.IsNullOrWhiteSpace(input))
            {
                return;
            }

            tbInput.Text = string.Empty;
            InputFinished?.Invoke(this, input);
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
            //TextRange range = new TextRange(tbInput.Document.ContentStart, tbInput.Document.ContentEnd);
            //range.ApplyPropertyValue(RichTextBox.FontSizeProperty, 16.0);
        }

        private void HandleOutputPreviewKeyDown(object sender, KeyEventArgs e)
        {
            char character = KeyUtils.GetCharFromKey(e.Key);

            if (character < 32 || character > 126)
            {
                return;
            }

            e.Handled = true;
            tbInput.Text += char.ToLower(character);
            tbInput.CaretIndex = tbInput.Text.Length;
            FocusInput();
        }

        private void HandleScrollSpeed(object sender, MouseWheelEventArgs e)
        {
            try
            {
                if (!(sender is DependencyObject))
                    return;

                ScrollViewer scrollViewer = GetScrollViewer((DependencyObject)sender);
                FlowDocumentScrollViewer lbHost = sender as FlowDocumentScrollViewer;

                if (scrollViewer == null || lbHost == null)
                {
                    throw new NotSupportedException("ScrollSpeed Attached Property is not attached to an element containing a ScrollViewer.");
                }

                double scrollSpeed = 5;
                double offset = scrollViewer.VerticalOffset - (e.Delta * scrollSpeed / 6);

                if (offset < 0)
                {
                    scrollViewer.ScrollToVerticalOffset(0);
                }
                else if (offset > scrollViewer.ExtentHeight)
                {
                    scrollViewer.ScrollToVerticalOffset(scrollViewer.ExtentHeight);
                }
                else
                {
                    scrollViewer.ScrollToVerticalOffset(offset);
                }

                e.Handled = true;
            }
            catch (Exception exception)
            {
                // TODO: logging
            }
        }

        private static ScrollViewer GetScrollViewer(DependencyObject obj)
        {
            if (obj is ScrollViewer viewer)
            {
                return viewer;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                var result = GetScrollViewer(child);

                if (result == null)
                {
                    continue;
                }

                return result;
            }

            return null;
        }
    }
}