using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.Streaming.Text
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

        public void SetBoldText()
        {
            WriteSgrAnciEscapeSequence("1");
        }

        public void SetItalicText()
        {
            WriteSgrAnciEscapeSequence("3");
        }

        public void SetUnderlineText()
        {
            WriteSgrAnciEscapeSequence("4");
        }

        public void SetCrossedOutText()
        {
            WriteSgrAnciEscapeSequence("9");
        }

        public void SetForegroundTextColor(ConsoleColor color)
        {
            string code = GetForegroundColorCode(color);

            if (code == null)
            {
                return;
            }

            WriteSgrAnciEscapeSequence(code);
        }

        public void SetForegroundTextColor(int red, int green, int blue)
        {
            string code = GetForegroundColorCode(red, green, blue);
            WriteSgrAnciEscapeSequence(code);
        }

        public void SetBackgroundTextColor(ConsoleColor color)
        {
            string code = GetBackgroundColorCode(color);

            if (code == null)
            {
                return;
            }

            WriteSgrAnciEscapeSequence(code);
        }

        public void ResetFormatting()
        {
            WriteSgrAnciEscapeSequence(string.Empty);
        }

        public Task SetBoldTextAsync()
        {
            return WriteSgrAnciEscapeSequenceAsync("1");
        }

        public Task SetItalicTextAsync()
        {
            return WriteSgrAnciEscapeSequenceAsync("3");
        }

        public Task SetUnderlineTextAsync()
        {
            return WriteSgrAnciEscapeSequenceAsync("4");
        }

        public Task SetCrossedOutTextAsync()
        {
            return WriteSgrAnciEscapeSequenceAsync("9");
        }

        public Task SetForegroundTextColorAsync(ConsoleColor color)
        {
            string code = GetForegroundColorCode(color);

            if (code == null)
            {
                return Task.CompletedTask;
            }

            return WriteSgrAnciEscapeSequenceAsync(code);
        }

        public Task SetForegroundTextColorAsync(int red, int green, int blue)
        {
            string code = GetForegroundColorCode(red, green, blue);
            return WriteSgrAnciEscapeSequenceAsync(code);
        }

        public Task SetBackgroundTextColorAsync(ConsoleColor color)
        {
            string code = GetBackgroundColorCode(color);

            if (code == null)
            {
                return Task.CompletedTask;
            }

            return WriteSgrAnciEscapeSequenceAsync(code);
        }

        public Task ResetFormattingAsync()
        {
            return WriteSgrAnciEscapeSequenceAsync(string.Empty);
        }

        private string GetForegroundColorCode(ConsoleColor color)
        {
            switch (color)
            {
                case ConsoleColor.Black:
                    return "30";

                case ConsoleColor.Red:
                case ConsoleColor.DarkRed:
                    return "31";

                case ConsoleColor.Green:
                case ConsoleColor.DarkGreen:
                    return "32";

                case ConsoleColor.Yellow:
                case ConsoleColor.DarkYellow:
                    return "33";

                case ConsoleColor.Blue:
                case ConsoleColor.DarkBlue:
                    return "34";

                case ConsoleColor.Magenta:
                case ConsoleColor.DarkMagenta:
                    return "35";

                case ConsoleColor.Cyan:
                case ConsoleColor.DarkCyan:
                    return "36";

                case ConsoleColor.White:
                    return "37";

                default:
                    return null;
            }
        }

        private string GetForegroundColorCode(int red, int green, int blue)
        {
            return $"38;2;{red};{green};{blue}";
        }

        private string GetBackgroundColorCode(ConsoleColor color)
        {
            switch (color)
            {
                case ConsoleColor.Black:
                    return "40";

                case ConsoleColor.Red:
                case ConsoleColor.DarkRed:
                    return "41";

                case ConsoleColor.Green:
                case ConsoleColor.DarkGreen:
                    return "42";

                case ConsoleColor.Yellow:
                case ConsoleColor.DarkYellow:
                    return "43";

                case ConsoleColor.Blue:
                case ConsoleColor.DarkBlue:
                    return "44";

                case ConsoleColor.Magenta:
                case ConsoleColor.DarkMagenta:
                    return "45";

                case ConsoleColor.Cyan:
                case ConsoleColor.DarkCyan:
                    return "46";

                case ConsoleColor.White:
                    return "47";
                
                default:
                    return null;
                    
            }
        }

        private void WriteSgrAnciEscapeSequence(string code)
        {
            Write($"\u001b[{code}m");
        }

        private Task WriteSgrAnciEscapeSequenceAsync(string code)
        {
            return WriteAsync($"\u001b[{code}m");
        }
    }
}
