using System;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Fallback.PowerShell
{
    public interface IPowerShellResultBuilder
    {
        bool ContainError { get; }

        bool Start();

        void SetRequireCustomErrorOutput();

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
