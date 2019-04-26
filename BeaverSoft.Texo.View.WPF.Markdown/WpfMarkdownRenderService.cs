using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using BeaverSoft.Texo.Core.Services;
using BeaverSoft.Texo.Core.Streaming.Text;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.View.WPF.Markdown
{
    public class WpfMarkdownRenderService : IWpfRenderService
    {
        private readonly Regex coloringRegex = new Regex(@"\u001b\[(?<code>\d*)m", RegexOptions.Compiled);
        private readonly IMarkdownService markdown;

        private Brush backgroundBrush = Brushes.Transparent;
        private Brush foregroundBrush = Brushes.White; // TODO: [P1] Solve this
        private FontStyle fontStyle = FontStyles.Normal;
        private FontWeight fontWeight = FontWeights.Normal;
        private TextDecorationCollection decorations = new TextDecorationCollection();
        private Paragraph streamedParagraph;
        private Run streamedScrollTreshold;
        private Action streamedContinueWith;

        public WpfMarkdownRenderService(IMarkdownService markdown)
        {
            this.markdown = markdown;
        }

        public Section Render(IItem item)
        {
            if (item.Format != TextFormatEnum.Markdown
                && item.Format != TextFormatEnum.Model)
            {
                return new Section(new Paragraph(BuildPlainText(item.Text)));
            }

            return BuildItem(item);
        }

        public Section StartStreamRender(TextStreamItem streamItem, Action continueWith)
        {
            ITextStream stream = streamItem.Stream;

            if (streamItem.OuterTask?.IsCompleted == true)
            {
                continueWith();
                return new Section(new Paragraph(BuildPlainText(stream.ReadFromBeginningToEnd())));
            }

            streamedContinueWith = continueWith;
            streamedScrollTreshold = new Run(string.Empty);
            streamedScrollTreshold.Loaded += NewText_Loaded;
            streamedParagraph = new Paragraph(streamedScrollTreshold);
            stream.StreamModified += Stream_StreamModified;
            stream.StreamCompleted += Stream_StreamCompleted;

            return new Section(streamedParagraph);
        }

        private void Stream_StreamCompleted(object sender, EventArgs e)
        {
            if (streamedParagraph == null)
            {
                return;
            }

            if (!streamedParagraph.Dispatcher.CheckAccess())
            {
                streamedParagraph.Dispatcher.Invoke(() => Stream_StreamCompleted(sender, e));
            }

            streamedParagraph = null;

            if (streamedScrollTreshold != null)
            {
                streamedScrollTreshold.Loaded -= NewText_Loaded;
                streamedScrollTreshold = null;
            }

            ITextStream stream = (ITextStream)sender;
            stream.StreamModified -= Stream_StreamModified;
            stream.StreamCompleted -= Stream_StreamCompleted;
            stream.Close();

            streamedContinueWith?.Invoke();
            streamedContinueWith = null;
        }

        private void Stream_StreamModified(object sender, EventArgs e)
        {
            if (streamedParagraph == null)
            {
                return;
            }

            ITextStream stream = (ITextStream)sender;
            string text = stream.ReadFromBeginningToEnd();

            if (streamedParagraph.Dispatcher.CheckAccess())
            {
                RenderModifiedStream(text);
            }
            else
            {
                streamedParagraph.Dispatcher.Invoke(() => RenderModifiedStream(text));
            }
        }

        private void RenderModifiedStream(string text)
        {
            if (streamedParagraph == null)
            {
                return;
            }

            Span newText = BuildPlainText(text);
            streamedParagraph.Inlines.Clear();
            streamedParagraph.Inlines.AddRange(new Inline[] { newText, streamedScrollTreshold });
        }

        private void NewText_Loaded(object sender, RoutedEventArgs e)
        {
            Run text = (Run)sender;
            text.BringIntoView();
        }

        private Section BuildItem(IItem item)
        {
            Section itemSection = BuildMarkdown(item.Text);
            itemSection.Tag = item;
            return itemSection;
        }

        private Section BuildMarkdown(string markdownText)
        {
            FlowDocument itemDocument = Markdig.Wpf.Markdown.ToFlowDocument(markdownText, markdown.Pipeline);
            Section itemSection = new Section();
            List<Block> blocks = itemDocument.Blocks.ToList();

            foreach (Block block in blocks)
            {
                itemSection.Blocks.Add(block);
            }

            return itemSection;
        }

        private Run CreateRun(string text)
        {
            return new Run(text)
            {
                Foreground = foregroundBrush,
                Background = backgroundBrush,
                FontStyle = fontStyle,
                FontWeight = fontWeight,
                TextDecorations = decorations.Clone()
            };
        }

        private void ResetFormatting()
        {
            backgroundBrush = Brushes.Transparent;
            foregroundBrush = Brushes.White;
            fontStyle = FontStyles.Normal;
            fontWeight = FontWeights.Normal;
            decorations.Clear();
        }

        private void UpdateFormatting(string value)
        {
            switch (value)
            {
                case "0":
                case "":
                    ResetFormatting();
                    break;

                case "1":
                    fontWeight = FontWeights.Bold;
                    break;

                case "3":
                    fontStyle = FontStyles.Italic;
                    break;

                case "4":
                    decorations.Add(TextDecorations.Underline);
                    break;

                case "9":
                    decorations.Add(TextDecorations.Strikethrough);
                    break;

                case "30":
                    foregroundBrush = Brushes.Black;
                    break;

                case "31":
                    foregroundBrush = Brushes.Red;
                    break;

                case "32":
                    foregroundBrush = Brushes.Green;
                    break;

                case "33":
                    foregroundBrush = Brushes.Yellow;
                    break;

                case "34":
                    foregroundBrush = Brushes.Blue;
                    break;

                case "35":
                    foregroundBrush = Brushes.Magenta;
                    break;

                case "36":
                    foregroundBrush = Brushes.Cyan;
                    break;

                case "37":
                    foregroundBrush = Brushes.White;
                    break;

                case "40":
                    backgroundBrush = Brushes.Black;
                    break;

                case "41":
                    backgroundBrush = Brushes.Red;
                    break;

                case "42":
                    backgroundBrush = Brushes.Green;
                    break;

                case "43":
                    backgroundBrush = Brushes.Yellow;
                    break;

                case "44":
                    backgroundBrush = Brushes.Blue;
                    break;

                case "45":
                    backgroundBrush = Brushes.Magenta;
                    break;

                case "46":
                    backgroundBrush = Brushes.Cyan;
                    break;

                case "47":
                    backgroundBrush = Brushes.White;
                    break;
            }
        }

        private Span BuildPlainText(string text)
        {
            Span container = new Span();
            int index = 0;
            Match match;

            ResetFormatting();

            while ((match = coloringRegex.Match(text, index)).Success)
            {
                if (index < match.Index)
                {
                    container.Inlines.Add(CreateRun(text.Substring(index, match.Index - index)));
                }

                UpdateFormatting(match.Groups["code"].Value);
                index = match.Index + match.Length;
            }

            if (index < text.Length)
            {
                container.Inlines.Add(CreateRun(text.Substring(index, text.Length - index)));
            }

            return container;
        }
    }
}
