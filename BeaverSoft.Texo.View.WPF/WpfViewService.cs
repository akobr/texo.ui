using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using BeaverSoft.Texo.Core.Actions;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Input;
using BeaverSoft.Texo.Core.Input.History;
using BeaverSoft.Texo.Core.Markdown.Builder;
using BeaverSoft.Texo.Core.Runtime;
using BeaverSoft.Texo.Core.View;
using Markdig.Wpf;
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
        private readonly IActionUrlParser actionParser;

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
            actionParser = new ActionUrlParser();

            currentPrompt = DEFAULT_PROMPT;
            currentTitle = DEFAULT_TITLE;
        }

        public void SetInput(string input)
        {
            control.SetInput(input.Trim());
        }

        public void AddInput(string append)
        {
            append = append.Trim();
            string input = control.GetInput();

            if (string.IsNullOrEmpty(input))
            {
                control.SetInput(append);
                return;
            }

            if (input.EndsWith(" "))
            {
                control.SetInput(input + append);
                return;
            }

            control.SetInput($"{input} {append}");
        }

        public string GetNewInput()
        {
            throw new NotImplementedException();
        }

        public void Render(Input input, IImmutableList<IItem> items)
        {
            // TODO: [P2] Solve this by result processing pipeline
            if (input.Context.Key == CommandKeys.CLEAR)
            {
                control.EnableInput();
                control.SetHistoryCount(history.Count);
                return;
            }

            List<Section> sections = new List<Section>(items.Count);
            Item headerItem = BuildCommandHeaderItem(input);
            Section header = renderer.Render(headerItem);
            header.Loaded += HandleLastSectionLoaded;
            sections.Add(header);

            if (items != null
                && items.Count > 0
                && (items.Count != 1 || !string.IsNullOrWhiteSpace(items[0].Text)))
            {
                foreach (IItem item in items)
                {
                    sections.Add(renderer.Render(item));
                }
            }

            control.OutputDocument.Blocks.AddRange(sections);
            control.EnableInput();
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
            control.IntellisenceList.Visibility = Visibility.Collapsed;
            control.IntellisenceList.Items.Clear();

            if (items == null || items.Count < 1)
            {
                return;
            }

            foreach (IItem item in items)
            {
                Section itemSection = renderer.Render(item);
                RichTextBox box = new RichTextBox()
                {
                    IsReadOnly = true,
                    IsReadOnlyCaretVisible = false,
                    IsManipulationEnabled = false,
                    IsHitTestVisible = false,
                    Margin = new Thickness(4),
                    BorderThickness = new Thickness(0),
                    Background = Brushes.Transparent,
                };

                box.SetResourceReference(Control.ForegroundProperty, "SystemBaseHighColorBrush");
                box.Document = new FlowDocument();
                box.Document.Blocks.AddRange(itemSection.Blocks.ToList());
                control.IntellisenceList.Items.Add(new ListBoxItem() { Content = box, Tag = item });
            }

            control.IntellisenceList.Visibility = Visibility.Visible;
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

        private void OnLinkCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnLinkExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            string url = e.Parameter.ToString();
            IActionContext actionContext = actionParser.Parse(url);
            
            if (actionContext.Name == ActionNames.INPUT_UPDATE)
            {
                UpdateInput(actionContext);
                return;
            }

            executor.ExecuteAction(url);
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
            control.KeyScrolled += TexoCommandHistoryScrolled;
            control.IntellisenceItemExecuted += TexoIntellisenceItemExecuted;

            CommandBinding linkCommandBinding = new CommandBinding(Commands.Hyperlink, OnLinkExecuted, OnLinkCanExecute);
            control.CommandBindings.Add(linkCommandBinding);

            control.Prompt = currentPrompt;
            control.Title = currentTitle;
            BuildInitialFlowDocument();
        }

        private void TexoIntellisenceItemExecuted(object sender, EventArgs e)
        {
            ListBoxItem viewItem = (ListBoxItem)control.IntellisenceList.SelectedItem;
            IItem item = (IItem)viewItem.Tag;
            control.CloseIntellisence();

            if (item.Actions.Count < 1)
            {
                return;
            }

            ILink actionLink = item.Actions.First();
            IActionContext actionContext = actionParser.Parse(actionLink.Address.AbsoluteUri);

            if (actionContext == null)
            {
                return;
            }

            if (actionContext.Name == ActionNames.INPUT_UPDATE)
            {
                UpdateInput(actionContext);
                return;
            }

            executor.ExecuteAction(actionLink.Address.AbsoluteUri);
        }

        private void UpdateInput(IActionContext actionContext)
        {
            if (!actionContext.Arguments.ContainsKey(ActionParameters.INPUT))
            {
                return;
            }

            string currentInput = control.GetInput();
            string value = actionContext.Arguments[ActionParameters.INPUT];
            bool isAdd = string.IsNullOrWhiteSpace(currentInput) || char.IsWhiteSpace(currentInput[currentInput.Length - 1]);

            if (isAdd)
            {
                control.SetInput(currentInput + value);
            }
            else
            {
                int index = currentInput.LastIndexOf(" ") + 1;
                control.SetInput(currentInput.Substring(0, index) + value);
            }
        }

        private void TexoCommandHistoryScrolled(object sender, KeyScrollDirection direction)
        {
            IHistoryItem historyItem = CalculateNewHystoryItem(direction);

            if (historyItem == null)
            {
                return;
            }

            control.SetInput(historyItem.Input.ParsedInput.RawInput);
        }

        private IHistoryItem CalculateNewHystoryItem(KeyScrollDirection direction)
        {
            if (historyItem == null)
            {
                historyItem = history.GetLastInput();
            }
            else if (direction == KeyScrollDirection.Back)
            {
                if (historyItem.IsDeleted)
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
            else
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

            return historyItem;
        }

        private void TexoInputChanged(object sender, string input)
        {
            executor.PreProcess(input, input.Length);
        }

        private async void TexoInputFinished(object sender, string input)
        {
            control.CloseIntellisence();
            control.DisableInput();
            lastWorkingDirectory = workingDirectory;
            historyItem = null;
            await executor.ProcessAsync(input);
        }

        private Item BuildCommandHeaderItem(Input input)
        {
            MarkdownBuilder headerBuilder = new MarkdownBuilder();
            headerBuilder.WriteLine("***");

            if (input.Context.IsValid)
            {
                WriteInputTokens(input, headerBuilder);
            }
            else
            {
                headerBuilder.Write(input.ParsedInput.RawInput);
                headerBuilder.Write(" ");
            }

            string atPath = lastWorkingDirectory;
            if (atPath[atPath.Length - 1] == '\\')
            {
                atPath += '\\';
            }

            //headerBuilder.WriteLine();
            //headerBuilder.Blockquotes(atPath);
            headerBuilder.Italic($"[{atPath}]");
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

        void IMessageBusRecipient<PromptUpdateMessage>.ProcessMessage(PromptUpdateMessage message)
        {
            if (control != null && !control.Dispatcher.CheckAccess())
            {
                control.Dispatcher.InvokeAsync(() => SetPrompt(message.Prompt));
                return;
            }

            SetPrompt(message.Prompt);
        }

        public void ProcessMessage(IVariableUpdatedMessage message)
        {
            if (control != null && !control.Dispatcher.CheckAccess())
            {
                control.Dispatcher.InvokeAsync(() => ProcessMessage(message));
                return;
            }

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
                SetPrompt("tu");
            }
        }

        void IMessageBusRecipient<IClearViewOutputMessage>.ProcessMessage(IClearViewOutputMessage message)
        {
            if (!control.Dispatcher.CheckAccess())
            {
                control.Dispatcher.InvokeAsync(() => control.OutputDocument.Blocks.Clear());
                return;
            }

            control.OutputDocument.Blocks.Clear();
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

        private void BuildInitialFlowDocument()
        {
            control.OutputDocument.Blocks.Add(new Paragraph(new Run(DEFAULT_TITLE))
            {
                FontSize = 14
            });

            control.OutputDocument.Blocks.Add(new Paragraph(new Run("Welcome in smart command line..."))
            {
                FontSize = 12,
                FontStyle = FontStyles.Italic,
                TextAlignment = TextAlignment.Left,
                Foreground = Brushes.DimGray
            });
        }

        private static void WriteInputTokens(Input input, IMarkdownBuilder headerBuilder)
        {
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
        }
    }
}
