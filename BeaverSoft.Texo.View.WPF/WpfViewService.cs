using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Input;
using BeaverSoft.Texo.Core.Runtime;
using BeaverSoft.Texo.Core.View;
using StrongBeaver.Core;
using StrongBeaver.Core.Messaging;

namespace BeaverSoft.Texo.View.WPF
{
    public class WpfViewService : IViewService, IInitialisable<TexoControl>
    {
        private const string DEFAULT_PROMPT = "tu>";
        private const string DEFAULT_TITLE = "Texo UI";

        private readonly IWpfRenderService renderer;

        private IExecutor executor;
        private TexoControl control;
        private bool showWorkingPathAsPrompt;
        private string currentPrompt;
        private string currentTitle;

        private int historyIndex;
        private string lastInput;

        public WpfViewService(IWpfRenderService renderer)
        {
            this.renderer = renderer;
            currentPrompt = DEFAULT_PROMPT;
            currentTitle = DEFAULT_TITLE;
        }

        public void Render(Input input)
        {
            // TODO
        }

        public void Render(IImmutableList<IItem> items)
        {
            List<Section> sections = new List<Section>(items.Count);
            Item headerItem = Item.Markdown($"***{Environment.NewLine}=={lastInput}==");
            Section header = renderer.Render(headerItem);
            header.Loaded += HandleLastSectionLoaded;
            sections.Add(header);

            foreach (IItem item in items)
            {
                sections.Add(renderer.Render(item));
            }

            control.OutputDocument.Document.Blocks.AddRange(sections);
            control.InputBox.IsReadOnlyCaretVisible = false;
        }

        private void HandleLastSectionLoaded(object sender, RoutedEventArgs e)
        {
            Section section = (Section)sender;
            section.Loaded -= HandleLastSectionLoaded;
            section.BringIntoView();
        }

        public void RenderIntellisence(IImmutableList<IItem> items)
        {
            throw new NotImplementedException();
        }

        public void RenderProgress(IProgress progress)
        {
            throw new NotImplementedException();
        }

        public void Update(string key, IItem item)
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            // TODO
        }

        public void Dispose()
        {
            // TODO
        }

        public void Initialise(IExecutor context)
        {
            executor = context;
        }

        public void Initialise(TexoControl context)
        {
            control = context;
            InitialiseControl();
        }

        private void InitialiseControl()
        {
            control.InputChanged += TexoInputChanged;
            control.InputFinished += TexoInputFinished;
            control.CommandHistoryScrolled += TexoCommandHistoryScrolled;

            control.Prompt = currentPrompt;
            control.Title = currentTitle;
            control.OutputDocument.Document = BuildInitialFlowDocument();
        }

        private void TexoCommandHistoryScrolled(object sender, HistoryScrollDirection direction)
        {
            IImmutableList<string> history = executor.GetPreviousCommands();

            if (direction == HistoryScrollDirection.Back)
            {
                historyIndex++;

                if (historyIndex >= history.Count)
                {
                    historyIndex = history.Count - 1;
                    return;
                }
            }
            else
            {
                historyIndex--;

                if (historyIndex < 0)
                {
                    historyIndex = 0;
                    return;
                }
            }

            int newIndex = history.Count - 1 - historyIndex;
            TextRange range = new TextRange(control.InputBox.Document.ContentStart, control.InputBox.Document.ContentEnd);
            range.Text = history[newIndex];
            control.InputBox.CaretPosition = control.InputBox.Document.ContentEnd;
        }

        private void TexoInputChanged(object sender, string input)
        {
            executor.PreProcess(input);
        }

        private void TexoInputFinished(object sender, string input)
        {
            lastInput = input;
            control.InputBox.IsReadOnlyCaretVisible = true;

            TextRange range = new TextRange(
                control.InputBox.Document.ContentStart,
                control.InputBox.Document.ContentEnd);
            range.Text = string.Empty;

            historyIndex = 0;
            executor.Process(input);
        }

        void IMessageBusRecipient<ISettingUpdatedMessage>.ProcessMessage(ISettingUpdatedMessage message)
        {
            showWorkingPathAsPrompt = message.Configuration.Ui.ShowWorkingPathAsPrompt;
            string currentDirectory = message.Configuration.Environment.Variables[VariableNames.CURRENT_DIRECTORY];

            if (showWorkingPathAsPrompt)
            {
                SetPrompt(currentDirectory);
                SetTitle(DEFAULT_TITLE);
            }
            else
            {
                SetPrompt(message.Configuration.Ui.Prompt);
                SetTitle(currentDirectory);
            }
        }

        void IMessageBusRecipient<IVariableUpdatedMessage>.ProcessMessage(IVariableUpdatedMessage message)
        {
            if (message.Name != VariableNames.CURRENT_DIRECTORY)
            {
                return;
            }

            if (showWorkingPathAsPrompt)
            {
                SetPrompt(message.NewValue);
            }
            else
            {
                SetTitle(message.NewValue);
            }
        }

        private void SetPrompt(string prompt)
        {
            currentPrompt = $"{prompt}>";

            if (control == null)
            {
                return;
            }

            control.Prompt = currentPrompt;
        }

        private void SetTitle(string title)
        {
            currentTitle = title;

            if (control == null)
            {
                return;
            }

            control.Title = currentTitle;
        }

        private static FlowDocument BuildInitialFlowDocument()
        {
            FlowDocument document = new FlowDocument();

            document.Blocks.Add(new Paragraph(new Run(DEFAULT_TITLE))
            {
                FontSize = 14
            });

            document.Blocks.Add(new Paragraph(new Run("Welcome in smart command line..."))
            {
                FontSize = 12,
                FontStyle = FontStyles.Italic,
                TextAlignment = TextAlignment.Left,
                Foreground = Brushes.DimGray
            });

            return document;
        }
    }
}
