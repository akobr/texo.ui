using System;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Fallback.PowerShell
{
    public interface IPowerShellResultBuilder
    {
        void StartItem();

        void Write(string text);

        void Write(string text, ConsoleColor foreground, ConsoleColor background);

        void WriteLine(string text);

        void WriteVerboseLine(string text);

        void WriteDebugLine(string text);

        void WriteWarningLine(string text);

        void WriteErrorLine(string text);

        Item FinishItem();
    }
}
