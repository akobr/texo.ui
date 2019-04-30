using BeaverSoft.Texo.Core.Streaming.Text;
using BeaverSoft.Texo.Core.View;
using System;
using System.Linq;
using System.Text;

namespace BeaverSoft.Texo.Fallback.PowerShell
{
    class PowerShellResultStreamBuilder : IPowerShellResultBuilder
    {
        private ReportableStream stream;
        private bool containError;
        private bool customErrorOutput;
        private FormattableStreamWriter writer;

        public bool ContainError => containError;

        public ReportableStream Stream => stream;

        public bool Start()
        {
            customErrorOutput = false;
            containError = false;

            stream = new ReportableStream();
            writer = new FormattableStreamWriter(stream.Stream, Encoding.UTF8, 1024, true);
            return true;
        }

        public IStreamedItem BuildStreamItem()
        {
            return new StreamedItem(stream);
        }

        public Item Finish()
        {
            writer.Dispose();
            writer = null;
            return Item.Empty;
        }

        public void SetRequireCustomErrorOutput()
        {
            customErrorOutput = true;
        }

        public void Write(string text)
        {
            writer.Write(text);
            writer.Flush();
        }

        public void Write(string text, ConsoleColor foreground, ConsoleColor background)
        {
            if (foreground != ConsoleColor.White)
            {
                writer.SetForegroundTextColor(foreground);
            }

            if (background != ConsoleColor.Black)
            {
                writer.SetBackgroundTextColor(background);
            }

            writer.Write(text);
            writer.ResetFormatting();
            writer.Flush();
        }

        public void WriteDebugLine(string text)
        {
            WriteColoredLine(text, ConsoleColor.Magenta);
        }

        public void WriteErrorLine(string text)
        {
            containError = true;

            if (customErrorOutput)
            {
                if (text.Contains('\r') || text.Contains('\n'))
                {
                    WriteColored(text, 255, 160, 122);
                    writer.Flush();
                }
                else
                {
                    WriteColored(text, 255, 160, 122);
                    WriteLine();
                    writer.Flush();
                }
            }
            else
            {
                WriteColoredLine(text, ConsoleColor.Red);
            }
        }

        public void WriteLine(string text)
        {
            writer.WriteLine(text);
            writer.Flush();
        }

        public void WriteLine()
        {
            writer.WriteLine();
        }

        public void WriteVerboseLine(string text)
        {
            WriteColoredLine(text, ConsoleColor.Green);
        }

        public void WriteWarningLine(string text)
        {
            WriteColoredLine(text, ConsoleColor.Yellow);
        }

        private void WriteColored(string text, ConsoleColor color)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            writer.SetForegroundTextColor(color);
            writer.Write(text);
            writer.ResetFormatting();
        }

        private void WriteColored(string text, int red, int green, int blue)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            writer.SetForegroundTextColor(red, green, blue);
            writer.Write(text);
            writer.ResetFormatting();
        }

        private void WriteColoredLine(string text, ConsoleColor color)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                writer.WriteLine();
                writer.Flush();
                return;
            }

            writer.SetForegroundTextColor(color);
            writer.Write(text);
            writer.ResetFormatting();
            writer.WriteLine();
            writer.Flush();
        }
    }
}
