using System;
using System.Text;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Fallback.PowerShell
{
    public class PowerShellResultPlainBuilder : IPowerShellResultBuilder
    {
        private StringBuilder builder;
        private bool containError;

        public bool ContainError => containError;

        public Item FinishItem()
        {
            return Item.Plain(builder.ToString());
        }

        public void StartItem()
        {
            containError = false;
            builder = new StringBuilder();
        }

        public void Write(string text)
        {
            builder.Append(text);
        }

        public void Write(string text, ConsoleColor foreground, ConsoleColor background)
        {
            builder.Append(text);
        }

        public void WriteDebugLine(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                builder.AppendLine();
            }

            builder.AppendLine($"\u001b[35m{text}\u001b[m");
        }

        public void WriteErrorLine(string text)
        {
            containError = true;

            if (string.IsNullOrWhiteSpace(text))
            {
                builder.AppendLine();
            }

            builder.AppendLine($"\u001b[31m{text}\u001b[m");
        }

        public void WriteLine(string text)
        {
            builder.AppendLine(text);
        }

        public void WriteVerboseLine(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                builder.AppendLine();
            }

            builder.AppendLine($"\u001b[32m{text}\u001b[m");
        }

        public void WriteWarningLine(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                builder.AppendLine();
            }

            builder.AppendLine($"\u001b[33m{text}\u001b[m");
        }
    }
}
