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

        public Item Finish()
        {
            return Item.Plain(builder.ToString());
        }

        public void SetRequireCustomOutput()
        {
            // no operation
        }

        public bool Start()
        {
            containError = false;
            builder = new StringBuilder();
            return false;
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

            builder.AppendLine(text);
        }

        public void WriteErrorLine(string text)
        {
            containError = true;

            if (string.IsNullOrWhiteSpace(text))
            {
                builder.AppendLine();
            }

            builder.AppendLine(text);
        }

        public void WriteLine(string text)
        {
            builder.AppendLine(text);
        }

        public void WriteLine()
        {
            builder.AppendLine();
        }

        public void WriteVerboseLine(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                builder.AppendLine();
            }

            builder.AppendLine(text);
        }

        public void WriteWarningLine(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                builder.AppendLine();
            }

            builder.AppendLine(text);
        }
    }
}
