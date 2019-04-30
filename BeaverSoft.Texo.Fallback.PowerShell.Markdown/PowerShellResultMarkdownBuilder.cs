using System;
using BeaverSoft.Texo.Core.Markdown.Builder;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Fallback.PowerShell.Markdown
{
    public class PowerShellResultMarkdownBuilder : IPowerShellResultBuilder
    {
        private IMarkdownBuilder markdown;
        private bool containError;

        public bool ContainError => containError;

        public Item Finish()
        {
            Item result = Item.Markdown(markdown.ToString());

            markdown = null;
            return result;
        }

        public void SetRequireCustomOutput()
        {
            // no operation
        }

        public bool Start()
        {
            containError = false;
            markdown = new MarkdownBuilder();
            return false;
        }

        public void Write(string text)
        {
            markdown.Write(text);
        }

        public void Write(string text, ConsoleColor foreground, ConsoleColor background)
        {
            markdown.Write(text);
        }

        public void WriteDebugLine(string text)
        {
            markdown.Italic(text);
            markdown.WriteLine();
        }

        public void WriteErrorLine(string text)
        {
            containError = true;
            markdown.Blockquotes(text);
        }

        public void WriteLine(string text)
        {
            markdown.WriteLine(text);
        }

        public void WriteLine()
        {
            markdown.WriteLine();
        }

        public void WriteVerboseLine(string text)
        {
            markdown.Italic(text);
            markdown.WriteLine();
        }

        public void WriteWarningLine(string text)
        {
            markdown.Blockquotes(text);
        }
    }
}
