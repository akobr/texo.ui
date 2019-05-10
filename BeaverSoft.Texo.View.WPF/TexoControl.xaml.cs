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
        private bool isInputDisabled;

        public static readonly DependencyProperty TextBaseColorProperty =
            DependencyProperty.Register(nameof(TextBaseColor), typeof(Color), typeof(TexoControl));

        public static readonly DependencyProperty BackgroundBaseColorProperty =
            DependencyProperty.Register(nameof(BackgroundBaseColor), typeof(Color), typeof(TexoControl));

        public static readonly DependencyProperty BorderBaseColorProperty =
            DependencyProperty.Register(nameof(BorderBaseColor), typeof(Color), typeof(TexoControl));

        public static readonly DependencyProperty AccentColorProperty =
            DependencyProperty.Register(nameof(AccentColor), typeof(Color), typeof(TexoControl));

        public static readonly DependencyProperty ShadowsVisibilityProperty =
            DependencyProperty.Register(nameof(ShadowsVisibility), typeof(Visibility), typeof(TexoControl));

        public event EventHandler<string> InputChanged;
        public event EventHandler<string> InputFinished;
        public event EventHandler<KeyScrollDirection> KeyScrolled;
        public event EventHandler IntellisenseItemExecuted;

        public TexoControl()
        {
            InitializeComponent();
            docOutput.Document = new FlowDocument();
        }

        public Color TextBaseColor
        {
            get => (Color)GetValue(TextBaseColorProperty);
            set => SetValue(TextBaseColorProperty, value);
        }

        public Color BackgroundBaseColor
        {
            get => (Color)GetValue(BackgroundBaseColorProperty);
            set => SetValue(BackgroundBaseColorProperty, value);
        }

        public Color BorderBaseColor
        {
            get => (Color)GetValue(BorderBaseColorProperty);
            set => SetValue(BorderBaseColorProperty, value);
        }

        public Color AccentColor
        {
            get => (Color)GetValue(AccentColorProperty);
            set => SetValue(AccentColorProperty, value);
        }

        public Visibility ShadowsVisibility
        {
            get => (Visibility)GetValue(ShadowsVisibilityProperty);
            set => SetValue(ShadowsVisibilityProperty, value);
        }

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
            CancelProgress();
            tbInput.Foreground = Brushes.White; // TODO [P2] this should be based on theme
            //tbInput.IsReadOnly = false;
            isInputDisabled = false;
        }

        public void DisableInput()
        {
            isInputDisabled = true;
            progress.IsIndeterminate = true;
            tbInput.Foreground = Brushes.DarkRed;
            //tbInput.IsReadOnly = true;
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
            skipNextTextChange = true;
            tbInput.Text = string.Empty;
            CloseIntellisense();

            //TextRange range = new TextRange(
            //    tbInput.Document.ContentStart,
            //    tbInput.Document.ContentEnd);
            //range.Text = string.Empty;
        }

        public void TryFinishInput()
        {
            if (isInputDisabled)
            {
                return;
            }

            string input = GetInput();

            if (string.IsNullOrWhiteSpace(input))
            {
                return;
            }
                       
            EmptyInput();
            InputFinished?.Invoke(this, input);
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
            if (!Dispatcher.CheckAccess())
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

        public void CleanResults()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.InvokeAsync(() => CleanResults());
                return;
            }

            docOutput.Visibility = Visibility.Collapsed;
            docOutput.Document.Blocks.Clear();
            docOutput.Visibility = Visibility.Visible;
        }

        private void HandleInputTextChanged(object sender, TextChangedEventArgs e)
        {
            if (skipNextTextChange)
            {
                skipNextTextChange = false;
                return;
            }

            string input = GetInput();
            InputChanged?.Invoke(this, input);
        }

        private void HandleInputKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                return;
            }

            e.Handled = true;
            TryFinishInput();
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
                TryFinishInput();
                return;
            }

            if (e.Key == Key.Back)
            {
                tbInput.Text = tbInput.Text.Substring(0, tbInput.Text.Length - 1);   
            }
            else if (e.Key != Key.Delete)
            {
                char character = KeyUtils.GetCharFromKey(e.Key);

                if (character < 32 || character > 126)
                {
                    return;
                }

                tbInput.Text += char.ToLower(character);
                tbInput.CaretIndex = tbInput.Text.Length;
            }

            tbInput.CaretIndex = tbInput.Text.Length;
            e.Handled = true;           
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
                // TODO: [P3] logging
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
            
        }

        private void HandleInputGotFocus(object sender, RoutedEventArgs e)
        {
            bInput.BorderBrush = SystemParameters.WindowGlassBrush;
            SwitchProgresForegroundAndBorderBrush();
        }

        private void HandleInputLostFocus(object sender, RoutedEventArgs e)
        {
            bInput.SetResourceReference(BorderBrushProperty, "InputBorderBrush");
            //bInput.BorderBrush = Brushes.Black;
            SwitchProgresForegroundAndBorderBrush();
            CloseIntellisense();
        }

        private void SwitchProgresForegroundAndBorderBrush()
        {
            var brush = progress.BorderBrush;
            progress.BorderBrush = progress.Foreground;
            progress.Foreground = brush;
        }

        private void ListIntellisense_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!(e.Source is ListBoxItem clickedItem))
            {
                return;
            }

            listIntellisense.SelectedItem = clickedItem;
            IntellisenseItemExecuted?.Invoke(this, new EventArgs());
        }
    }
}