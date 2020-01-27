using System;
using System.Text;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.View;
using BeaverSoft.Texo.Fallback.PowerShell.Transforming;

namespace BeaverSoft.Texo.Fallback.PowerShell
{
    public class PowerShellResultPlainBuilder : IPowerShellResultBuilder
    {
        private readonly StringBuilder builder;

        public PowerShellResultPlainBuilder()
        {
            builder = new StringBuilder();
        }

        public ValueTask StartAsync(InputModel inputModel)
        {
            builder.Clear();
            return new ValueTask();
        }

        public ValueTask<Item> FinishAsync()
        {
            return new ValueTask<Item>(Item.AsPlain(builder.ToString()));
        }

        public ValueTask WriteAsync(string text)
        {
            builder.Append(text);
            return new ValueTask();
        }

        public ValueTask WriteAsync(string text, ConsoleColor foreground, ConsoleColor? background = null)
        {
            builder.Append(text);
            return new ValueTask();
        }

        public ValueTask WriteLineAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                builder.AppendLine();
            }

            builder.AppendLine(text);
            return new ValueTask();
        }

        public ValueTask WriteLineAsync(string text, ConsoleColor foreground, ConsoleColor? background = null)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                builder.AppendLine();
            }

            builder.AppendLine(text);
            return new ValueTask();
        }

        public ValueTask WriteLineAsync()
        {
            builder.AppendLine();
            return new ValueTask();
        }
    }
}
