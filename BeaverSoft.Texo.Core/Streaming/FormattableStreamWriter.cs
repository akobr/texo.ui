using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Text;

namespace BeaverSoft.Texo.Core.Streaming
{
    // TODO: [P3] Move formating constants to value object
    public class FormattableStreamWriter : StreamWriter
    {
        public FormattableStreamWriter(Stream stream)
            : base(stream)
        {
            // no operation
        }

        public FormattableStreamWriter(string path)
            : base(path)
        {
            // no operation
        }

        public FormattableStreamWriter(Stream stream, Encoding encoding)
            : base(stream, encoding)
        {
            // no operation
        }

        public FormattableStreamWriter(string path, bool append)
            : base(path, append)
        {
            // no operation
        }

        public FormattableStreamWriter(Stream stream, Encoding encoding, int bufferSize)
            : base(stream, encoding, bufferSize)
        {
            // no operation
        }

        public FormattableStreamWriter(string path, bool append, Encoding encoding)
            : base(path, append, encoding)
        {
            // no operation
        }

        public FormattableStreamWriter(Stream stream, Encoding encoding, int bufferSize, bool leaveOpen)
            : base(stream, encoding, bufferSize, leaveOpen)
        {
            // no operation
        }

        public FormattableStreamWriter(string path, bool append, Encoding encoding, int bufferSize)
            : base(path, append, encoding, bufferSize)
        {
            // no operation
        }

        public void WriteLink(string text, string url)
        {
            WriteAnsiSgrEscapeSequence(AnsiSgrEscapeCodes.LINK_START);
            Write(text);
            Write(AnsiSgrEscapeCodes.LINK_SEPARATOR);
            Write(url);
            WriteAnsiSgrEscapeSequence(AnsiSgrEscapeCodes.LINK_END);
        }

        public void SetBoldText()
        {
            WriteAnsiSgrEscapeSequence(AnsiSgrEscapeCodes.BOLD);
        }

        public void SetItalicText()
        {
            WriteAnsiSgrEscapeSequence(AnsiSgrEscapeCodes.ITALIC);
        }

        public void SetUnderlineText()
        {
            WriteAnsiSgrEscapeSequence(AnsiSgrEscapeCodes.UNDERLINE);
        }

        public void SetCrossedOutText()
        {
            WriteAnsiSgrEscapeSequence(AnsiSgrEscapeCodes.CROSSED_OUT);
        }

        public void SetForegroundTextColor(ConsoleColor color)
        {
            string code = AnsiSgrEscapeCodes.GetForegroundColorCode(color);

            if (code == null)
            {
                return;
            }

            WriteAnsiSgrEscapeSequence(code);
        }

        public void SetForegroundTextColor(byte red, byte green, byte blue)
        {
            string code = AnsiSgrEscapeCodes.GetForegroundColorCode(red, green, blue);
            WriteAnsiSgrEscapeSequence(code);
        }

        public void SetBackgroundTextColor(ConsoleColor color)
        {
            string code = AnsiSgrEscapeCodes.GetBackgroundColorCode(color);

            if (code == null)
            {
                return;
            }

            WriteAnsiSgrEscapeSequence(code);
        }

        public void ResetFormatting()
        {
            WriteAnsiSgrEscapeSequence(string.Empty);
        }

        public Task SetBoldTextAsync()
        {
            return WriteAnsiSgrEscapeSequenceAsync(AnsiSgrEscapeCodes.BOLD);
        }

        public Task SetItalicTextAsync()
        {
            return WriteAnsiSgrEscapeSequenceAsync(AnsiSgrEscapeCodes.ITALIC);
        }

        public Task SetUnderlineTextAsync()
        {
            return WriteAnsiSgrEscapeSequenceAsync(AnsiSgrEscapeCodes.UNDERLINE);
        }

        public Task SetCrossedOutTextAsync()
        {
            return WriteAnsiSgrEscapeSequenceAsync(AnsiSgrEscapeCodes.CROSSED_OUT);
        }

        public Task SetForegroundTextColorAsync(ConsoleColor color)
        {
            string code = AnsiSgrEscapeCodes.GetForegroundColorCode(color);

            if (code == null)
            {
                return Task.CompletedTask;
            }

            return WriteAnsiSgrEscapeSequenceAsync(code);
        }

        public Task SetForegroundTextColorAsync(byte red, byte green, byte blue)
        {
            string code = AnsiSgrEscapeCodes.GetForegroundColorCode(red, green, blue);
            return WriteAnsiSgrEscapeSequenceAsync(code);
        }

        public Task SetBackgroundTextColorAsync(ConsoleColor color)
        {
            string code = AnsiSgrEscapeCodes.GetBackgroundColorCode(color);

            if (code == null)
            {
                return Task.CompletedTask;
            }

            return WriteAnsiSgrEscapeSequenceAsync(code);
        }

        public Task ResetFormattingAsync()
        {
            return WriteAnsiSgrEscapeSequenceAsync(string.Empty);
        }

        private void WriteAnsiSgrEscapeSequence(string code)
        {
            Write($"\u001b[{code}m");
        }

        private Task WriteAnsiSgrEscapeSequenceAsync(string code)
        {
            return WriteAsync($"\u001b[{code}m");
        }
    }
}
