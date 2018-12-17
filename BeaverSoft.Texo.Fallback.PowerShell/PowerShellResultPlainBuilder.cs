using System;
using System.Text;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Fallback.PowerShell
{
    public class PowerShellResultPlainBuilder : IPowerShellResultBuilder
    {
        private StringBuilder builder;

        public Item FinishItem()
        {
            return Item.Plain(builder.ToString());
        }

        public void StartItem()
        {
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
            builder.AppendLine(text);
        }

        public void WriteErrorLine(string text)
        {
            builder.AppendLine(text);
        }

        public void WriteLine(string text)
        {
            builder.AppendLine(text);
        }

        public void WriteVerboseLine(string text)
        {
            builder.AppendLine(text);
        }

        public void WriteWarningLine(string text)
        {
            builder.AppendLine(text);
        }
    }
}
