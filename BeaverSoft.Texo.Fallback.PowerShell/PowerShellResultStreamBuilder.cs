using BeaverSoft.Texo.Core.Streaming.Text;
using BeaverSoft.Texo.Core.View;
using System;

namespace BeaverSoft.Texo.Fallback.PowerShell
{
    class PowerShellResultStreamBuilder : IPowerShellResultBuilder
    {
        private TextStream stream;
        private bool containError;

        public bool ContainError => containError;

        public TextStream Stream => stream;

        public Item FinishItem()
        {
            return Item.Empty;
        }

        public void StartItem()
        {
            containError = false;
            stream = new TextStream();
        }

        public void Write(string text)
        {
            stream.Write(text);
            stream.Flush();
            stream.NotifyAboutChange();
        }

        public void Write(string text, ConsoleColor foreground, ConsoleColor background)
        {
            if (foreground != ConsoleColor.White)
            {
                stream.SetForegroundTextColor(foreground);
            }

            if (background != ConsoleColor.Black)
            {
                stream.SetBackgroundTextColor(background);
            }

            stream.Write(text);
            stream.ResetFormatting();
            stream.Flush();
            stream.NotifyAboutChange();
        }

        public void WriteDebugLine(string text)
        {
            WriteColorLine(text, ConsoleColor.Magenta);
        }

        public void WriteErrorLine(string text)
        {
            containError = true;
            WriteColorLine(text, ConsoleColor.Red);
        }

        public void WriteLine(string text)
        {
            stream.WriteLine(text);
            stream.Flush();
            stream.NotifyAboutChange();
        }

        public void WriteVerboseLine(string text)
        {
            WriteColorLine(text, ConsoleColor.Green);
        }

        public void WriteWarningLine(string text)
        {
            WriteColorLine(text, ConsoleColor.Yellow);
        }

        private void WriteColorLine(string text, ConsoleColor color)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                stream.Write(text);
                stream.WriteLine();
                stream.Flush();
                return;
            }

            stream.SetForegroundTextColor(color);
            stream.Write(text);
            stream.ResetFormatting();
            stream.WriteLine();
            stream.Flush();
            stream.NotifyAboutChange();
        }
    }
}
