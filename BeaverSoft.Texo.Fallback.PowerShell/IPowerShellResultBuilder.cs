using System;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.View;
using BeaverSoft.Texo.Fallback.PowerShell.Transforming;

namespace BeaverSoft.Texo.Fallback.PowerShell
{
    public interface IPowerShellResultBuilder
    {
        ValueTask StartAsync(InputModel inputModel);

        ValueTask WriteAsync(string text);

        ValueTask WriteAsync(string text, ConsoleColor foreground, ConsoleColor? background = null);

        ValueTask WriteLineAsync(string text);

        ValueTask WriteLineAsync(string text, ConsoleColor foreground, ConsoleColor? background = null);

        ValueTask WriteLineAsync();

        ValueTask<Item> FinishAsync();
    }
}
