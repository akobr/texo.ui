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
using BeaverSoft.Texo.Core.Input.History;
using BeaverSoft.Texo.Core.Markdown.Builder;
using BeaverSoft.Texo.Core.Path;
using BeaverSoft.Texo.Core.Runtime;
using BeaverSoft.Texo.Core.View;
using StrongBeaver.Core;
using StrongBeaver.Core.Messaging;

namespace BeaverSoft.Texo.View.WPF
{
    public class WpfViewService : IViewService, IPromptableViewService, IInitialisable<TexoControl>
    {
        private const string DEFAULT_PROMPT = "tu>";
        private const string DEFAULT_TITLE = "Texo UI";

        private readonly IWpfRenderService renderer;
        private readonly IFactory<IInputHistoryService> historyFactory;

        private IExecutor executor;
        private TexoControl control;
        private IInputHistoryService history;

        private bool showWorkingPathAsPrompt;
        private string currentPrompt;
        private string currentTitle;
        private string workingDirectory;
        private string lastWorkingDirectory;

        private IHistoryItem historyItem;

        public WpfViewService(IWpfRenderService renderer, IFactory<IInputHistoryService> historyFactory)
        {
            this.renderer = renderer;
            this.historyFactory = historyFactory;

            currentPrompt = DEFAULT_PROMPT;
            currentTitle = DEFAULT_TITLE;
        }

        public void Render(Input input, IImmutableList<IItem> items)
        {
            List<Section> sections = new List<Section>(items.Count);
            Item headerItem = BuildCommandHeaderItem(input);
            Section header = renderer.Render(headerItem);
            header.Loaded += HandleLastSectionLoaded;
            sections.Add(header);

            foreach (IItem item in items)
            {
                sections.Add(renderer.Render(item));
            }

            control.OutputDocument.Document.Blocks.AddRange(sections);
            control.InputBox.IsReadOnlyCaretVisible = false;
            control.SetHistoryCount(history.Count);
        }

        private void HandleLastSectionLoaded(object sender, RoutedEventArgs e)
        {
            Section section = (Section)sender;
            section.Loaded -= HandleLastSectionLoaded;
            section.BringIntoView();
        }

        public void RenderIntellisence(Input input, IImmutableList<IItem> items)
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
            history = historyFactory.Create();
        }

        public void Initialise(TexoControl context)
        {
            control = context;
            InitialiseControl();
        }

        public string GetNewInput()
        {
            throw new NotImplementedException();
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
            if (direction == HistoryScrollDirection.Back)
            {
                if (historyItem == null)
                {
                    historyItem = history.GetLastInput();
                }
                else if (historyItem.IsDeleted)
                {
                    while (historyItem != null && historyItem.IsDeleted)
                    {
                        historyItem = historyItem.Next;
                    }
                }
                else if (historyItem.HasPrevious)
                {
                    historyItem = historyItem.Previous;
                }
            }
            else if (historyItem != null)
            {
                if (historyItem.IsDeleted)
                {
                    while (historyItem != null && historyItem.IsDeleted)
                    {
                        historyItem = historyItem.Next;
                    }
                }
                else if (historyItem.HasNext)
                {
                    historyItem = historyItem.Next;
                }
            }

            if (historyItem == null)
            {
                return;
            }

            TextRange range = new TextRange(control.InputBox.Document.ContentStart, control.InputBox.Document.ContentEnd);
            range.Text = historyItem.Input.ParsedInput.RawInput;
            control.InputBox.CaretPosition = control.InputBox.Document.ContentEnd;
        }

        private void TexoInputChanged(object sender, string input)
        {
            //executor.PreProcess(input, input.Length);
        }

        private void TexoInputFinished(object sender, string input)
        {
            control.InputBox.IsReadOnlyCaretVisible = true;

            TextRange range = new TextRange(
                control.InputBox.Document.ContentStart,
                control.InputBox.Document.ContentEnd);
            range.Text = string.Empty;

            lastWorkingDirectory = workingDirectory;
            executor.Process(input);
        }

        private Item BuildCommandHeaderItem(Input input)
        {
            MarkdownBuilder headerBuilder = new MarkdownBuilder();
            headerBuilder.WriteLine("***");

            foreach (IToken token in input.Tokens)
            {
                switch (token.Type)
                {
                    case TokenTypeEnum.Query:
                        headerBuilder.Bold(token.Input);
                        break;

                    case TokenTypeEnum.Option:
                    case TokenTypeEnum.OptionList:
                        headerBuilder.Italic(token.Input);
                        break;

                    case TokenTypeEnum.Wrong:
                    case TokenTypeEnum.Unknown:
                        continue;

                    default:
                        headerBuilder.Write(token.Input);
                        break;
                }

                headerBuilder.Write(" ");
            }

            string atPath = lastWorkingDirectory;
            if (atPath[atPath.Length - 1] == '\\')
            {
                atPath += '\\';
            }

            headerBuilder.WriteLine($"at =={atPath}==");
            return Item.Markdown(headerBuilder.ToString());
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
            control?.SetVariableCount(message.Environment.Count);

            if (message.Name != VariableNames.CURRENT_DIRECTORY)
            {
                return;
            }

            workingDirectory = message.NewValue;

            if (showWorkingPathAsPrompt)
            {
                SetPrompt(workingDirectory);
            }
            else
            {
                SetTitle(workingDirectory);
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
