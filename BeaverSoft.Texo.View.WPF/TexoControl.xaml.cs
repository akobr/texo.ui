using BeaverSoft.Texo.Core.View;
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
        private bool skipNextTextChange;
        private string prompt;

        public TexoControl()
        {
            InitializeComponent();
            docOutput.Document = new FlowDocument();
        }

        public event EventHandler<string> InputChanged;
        public event EventHandler<string> InputFinished;
        public event EventHandler<KeyScrollDirection> KeyScrolled;
        public event EventHandler IntellisenseItemExecuted;

        public FlowDocument OutputDocument => docOutput.Document;

        public ListBox IntellisenseList => listIntellisense;

        public string Prompt
        {
            get => prompt;
            set
            {
                prompt = value;
                lbPrompt.Text = value;
            }
        }

        public string Title
        {
            get => lbTitle.Text;
            set => lbTitle.Text = value;
        }

        public bool IsIntellisenseOpened => listIntellisense.Visibility == Visibility.Visible;

        public void EnableInput()
        {
            tbInput.IsReadOnly = false;
            CancelProgress();
        }

        public void DisableInput()
        {
            progress.IsIndeterminate = true;
            tbInput.IsReadOnly = true;
        }

        public void CloseIntellisense()
        {
            listIntellisense.Visibility = Visibility.Collapsed;
        }

        public string GetInput()
        {
            return tbInput.Text;
            //TextRange range = new TextRange(tbInput.Document.ContentStart, tbInput.Document.ContentEnd);
            //return range.Text.Trim();
        }

        public void EmptyInput()
        {
            tbInput.Text = string.Empty;

            //TextRange range = new TextRange(
            //    tbInput.Document.ContentStart,
            //    tbInput.Document.ContentEnd);
            //range.Text = string.Empty;
        }

        public void SetInput(string input)
        {
            skipNextTextChange = true;
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

        public void SetProgress(string progressName, int progressValue)
        {
            if(!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => SetProgress(progressName, progressValue));
                return;
            }

            lbPrompt.Text = $"{progressName}>";

            if (progressValue < 0)
            {
                progress.IsIndeterminate = true;
                return;
            }

            progress.IsIndeterminate = false;
            progress.Value = Math.Min(progressValue, 100);
        }

        public void CancelProgress()
        {
            progress.Value = 0;
            progress.IsIndeterminate = false;
            lbPrompt.Text = prompt;
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
            if (skipNextTextChange)
            {
                skipNextTextChange = false;
                return;
            }

            string input = GetInput();

            if (!string.IsNullOrEmpty(input))
            {
                InputChanged?.Invoke(this, input);
            }
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
            if ((e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl))
                && e.KeyboardDevice.IsKeyDown(Key.Space))
            {
                e.Handled = true;
                InputChanged?.Invoke(this, GetInput());
                return;
            }

            if (e.Key == Key.Tab)
            {
                e.Handled = true;
                if (IsIntellisenseOpened)
                {
                    int selectedIndex = listIntellisense.SelectedIndex + 1;

                    if (selectedIndex >= listIntellisense.Items.Count)
                    {
                        selectedIndex = 0;
                    }

                    listIntellisense.SelectedIndex = selectedIndex;
                    listIntellisense.ScrollIntoView(listIntellisense.SelectedItem);
                }
                else
                {
                    InputChanged?.Invoke(this, GetInput());
                }

                return;
            }

            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                CloseIntellisense();
                return;
            }

            if (e.Key == Key.Enter
                && IsIntellisenseOpened
                && listIntellisense.SelectedIndex >= 0)
            {
                e.Handled = true;
                IntellisenseItemExecuted?.Invoke(this, new EventArgs());
                return;
            }

            if (e.Key == Key.Up)
            {
                e.Handled = true;

                if (IsIntellisenseOpened)
                {
                    if (listIntellisense.SelectedIndex > 0)
                    {
                        listIntellisense.SelectedIndex--;
                    }
                    else if(listIntellisense.SelectedIndex < 0)
                    {
                        listIntellisense.SelectedIndex = 0;
                    }

                    listIntellisense.ScrollIntoView(listIntellisense.SelectedItem);
                }
                else
                {
                    KeyScrolled?.Invoke(this, KeyScrollDirection.Back);
                }
                
                return;
            }

            if (e.Key == Key.Down)
            {
                e.Handled = true;

                if (IsIntellisenseOpened)
                {
                    if (listIntellisense.SelectedIndex < listIntellisense.Items.Count - 1)
                    {
                        listIntellisense.SelectedIndex++;
                        listIntellisense.ScrollIntoView(listIntellisense.SelectedItem);
                    }
                }
                else
                {
                    KeyScrolled?.Invoke(this, KeyScrollDirection.Forward);
                }

                return;
            }
        }

        private void HandleInputSelectionChanged(object sender, RoutedEventArgs e)
        {
            //TextRange range = new TextRange(tbInput.Document.ContentStart, tbInput.Document.ContentEnd);
            //range.ApplyPropertyValue(RichTextBox.FontSizeProperty, 16.0);
        }

        private void HandleOutputPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string input = GetInput();

                if (string.IsNullOrWhiteSpace(input))
                {
                    return;
                }

                tbInput.Text = string.Empty;
                InputFinished?.Invoke(this, input);
            }

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

        private void HandleIntellisenseMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listIntellisense.SelectedItem == null)
            {
                return;
            }

            IntellisenseItemExecuted?.Invoke(this, new EventArgs());
        }
    }
}