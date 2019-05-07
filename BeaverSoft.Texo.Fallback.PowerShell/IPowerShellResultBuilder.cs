using System;
using BeaverSoft.Texo.Core.View;
using BeaverSoft.Texo.Fallback.PowerShell.Transforming;

namespace BeaverSoft.Texo.Fallback.PowerShell
{
    public interface IPowerShellResultBuilder
    {
        bool ContainError { get; }

        bool Start(InputModel inputModel);

        void Write(string text);

        void Write(string text, ConsoleColor foreground, ConsoleColor background);

        void WriteLine(string text);

        void WriteLine();

        void WriteVerboseLine(string text);

        void WriteDebugLine(string text);

        void WriteWarningLine(string text);

        void WriteErrorLine(string text);

        Item Finish();
    }
}
