using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.Streaming.Text
{
    public class TextStream : ITextStream, IFormattableTextStream, IWritableTextStream, IDisposable
    {
        private const int DEFAULT_BUFFER_SIZE = 1024;

        private readonly ConcurrentMemoryStream stream;
        private readonly TextWriter writer;
        private readonly TextReader reader;

        public TextStream()
        {
            stream = new ConcurrentMemoryStream();
            writer = new StreamWriter(stream, Encoding.UTF8, DEFAULT_BUFFER_SIZE, true);
            reader = new StreamReader(stream, Encoding.UTF8, false, DEFAULT_BUFFER_SIZE, true);
        }

        public event EventHandler StreamModified;

        public event EventHandler StreamCompleted;

        public bool IsClosed => !stream.CanRead;

        public void NotifyAboutChange()
        {
            StreamModified?.Invoke(this, new EventArgs());
        }

        public void NotifyAboutCompletion()
        {
            StreamCompleted?.Invoke(this, new EventArgs());
        }

        public string ReadFromBeginningToEnd()
        {
            stream.SeekReading(0, SeekOrigin.Begin);
            return reader.ReadToEnd();
        }

        public string ReadLine()
        {
            return reader.ReadLine();
        }

        public string ReadToEnd()
        {
            return reader.ReadToEnd();
        }

        public Task<string> ReadFromBeginningToEndAsync()
        {
            stream.SeekReading(0, SeekOrigin.Begin);
            return reader.ReadToEndAsync();
        }

        public Task<string> ReadLineAsync()
        {
            return reader.ReadLineAsync();
        }

        public Task<string> ReadToEndAsync()
        {
            return reader.ReadToEndAsync();
        }

        public void Write(string text)
        {
            writer.Write(text);
        }

        public void WriteLine(string text)
        {
            writer.WriteLine(text);
        }

        public void WriteLine()
        {
            writer.WriteLine();
        }

        public void Flush()
        {
            writer.Flush();
        }

        public Task WriteAsync(string text)
        {
            return writer.WriteAsync(text);
        }

        public Task WriteLineAsync(string text)
        {
            return writer.WriteLineAsync(text);
        }

        public Task WriteLineAsync()
        {
            return writer.WriteLineAsync();
        }

        public Task FlushAsync()
        {
            return writer.FlushAsync();
        }

        public void Close()
        {
            writer.Close();
            reader.Close();
            stream.Close();
        }

        public void Dispose()
        {
            writer.Dispose();
            reader.Dispose();
            stream.Dispose();
        }

        public void SetBoldText()
        {
            WriteSgrAnciEscapeSequence(1);
        }

        public void SetItalicText()
        {
            WriteSgrAnciEscapeSequence(3);
        }

        public void SetUnderlineText()
        {
            WriteSgrAnciEscapeSequence(4);
        }

        public void SetCrossedOutText()
        {
            WriteSgrAnciEscapeSequence(9);
        }

        public void SetForegroundTextColor(ConsoleColor color)
        {
            switch (color)
            {
                case ConsoleColor.Black:
                    WriteSgrAnciEscapeSequence(30);
                    break;

                case ConsoleColor.Red:
                case ConsoleColor.DarkRed:
                    WriteSgrAnciEscapeSequence(31);
                    break;

                case ConsoleColor.Green:
                case ConsoleColor.DarkGreen:
                    WriteSgrAnciEscapeSequence(32);
                    break;

                case ConsoleColor.Yellow:
                case ConsoleColor.DarkYellow:
                    WriteSgrAnciEscapeSequence(33);
                    break;

                case ConsoleColor.Blue:
                case ConsoleColor.DarkBlue:
                    WriteSgrAnciEscapeSequence(34);
                    break;

                case ConsoleColor.Magenta:
                case ConsoleColor.DarkMagenta:
                    WriteSgrAnciEscapeSequence(35);
                    break;

                case ConsoleColor.Cyan:
                case ConsoleColor.DarkCyan:
                    WriteSgrAnciEscapeSequence(36);
                    break;

                case ConsoleColor.White:
                    WriteSgrAnciEscapeSequence(37);
                    break;
            }
        }

        public void SetBackgroundTextColor(ConsoleColor color)
        {
            switch (color)
            {
                case ConsoleColor.Black:
                    WriteSgrAnciEscapeSequence(40);
                    break;

                case ConsoleColor.Red:
                case ConsoleColor.DarkRed:
                    WriteSgrAnciEscapeSequence(41);
                    break;

                case ConsoleColor.Green:
                case ConsoleColor.DarkGreen:
                    WriteSgrAnciEscapeSequence(42);
                    break;

                case ConsoleColor.Yellow:
                case ConsoleColor.DarkYellow:
                    WriteSgrAnciEscapeSequence(43);
                    break;

                case ConsoleColor.Blue:
                case ConsoleColor.DarkBlue:
                    WriteSgrAnciEscapeSequence(44);
                    break;

                case ConsoleColor.Magenta:
                case ConsoleColor.DarkMagenta:
                    WriteSgrAnciEscapeSequence(45);
                    break;

                case ConsoleColor.Cyan:
                case ConsoleColor.DarkCyan:
                    WriteSgrAnciEscapeSequence(46);
                    break;

                case ConsoleColor.White:
                    WriteSgrAnciEscapeSequence(47);
                    break;
            }
        }

        public void ResetFormatting()
        {
            Write($"\u001b[m");
        }

        public Task SetBoldTextAsync()
        {
            return WriteSgrAnciEscapeSequenceAsync(1);
        }

        public Task SetItalicTextAsync()
        {
            return WriteSgrAnciEscapeSequenceAsync(3);
        }

        public Task SetUnderlineTextAsync()
        {
            return WriteSgrAnciEscapeSequenceAsync(4);
        }

        public Task SetCrossedOutTextAsync()
        {
            return WriteSgrAnciEscapeSequenceAsync(9);
        }

        public Task SetForegroundTextColorAsync(ConsoleColor color)
        {
            switch (color)
            {
                case ConsoleColor.Black:
                    return WriteSgrAnciEscapeSequenceAsync(30);

                case ConsoleColor.Red:
                case ConsoleColor.DarkRed:
                    return WriteSgrAnciEscapeSequenceAsync(31);

                case ConsoleColor.Green:
                case ConsoleColor.DarkGreen:
                    return WriteSgrAnciEscapeSequenceAsync(32);

                case ConsoleColor.Yellow:
                case ConsoleColor.DarkYellow:
                    return WriteSgrAnciEscapeSequenceAsync(33);

                case ConsoleColor.Blue:
                case ConsoleColor.DarkBlue:
                    return WriteSgrAnciEscapeSequenceAsync(34);

                case ConsoleColor.Magenta:
                case ConsoleColor.DarkMagenta:
                    return WriteSgrAnciEscapeSequenceAsync(35);

                case ConsoleColor.Cyan:
                case ConsoleColor.DarkCyan:
                    return WriteSgrAnciEscapeSequenceAsync(36);

                case ConsoleColor.White:
                    return WriteSgrAnciEscapeSequenceAsync(37);

                default:
                    return Task.CompletedTask;
            }
        }

        public Task SetBackgroundTextColorAsync(ConsoleColor color)
        {
            switch (color)
            {
                case ConsoleColor.Black:
                    return WriteSgrAnciEscapeSequenceAsync(40);

                case ConsoleColor.Red:
                case ConsoleColor.DarkRed:
                    return WriteSgrAnciEscapeSequenceAsync(41);

                case ConsoleColor.Green:
                case ConsoleColor.DarkGreen:
                    return WriteSgrAnciEscapeSequenceAsync(42);

                case ConsoleColor.Yellow:
                case ConsoleColor.DarkYellow:
                    return WriteSgrAnciEscapeSequenceAsync(43);

                case ConsoleColor.Blue:
                case ConsoleColor.DarkBlue:
                    return WriteSgrAnciEscapeSequenceAsync(44);

                case ConsoleColor.Magenta:
                case ConsoleColor.DarkMagenta:
                    return WriteSgrAnciEscapeSequenceAsync(45);

                case ConsoleColor.Cyan:
                case ConsoleColor.DarkCyan:
                    return WriteSgrAnciEscapeSequenceAsync(46);

                case ConsoleColor.White:
                    return WriteSgrAnciEscapeSequenceAsync(47);

                default:
                    return Task.CompletedTask;
            }
        }

        public Task ResetFormattingAsync()
        {
            return WriteAsync($"\u001b[m");
        }

        private void WriteSgrAnciEscapeSequence(byte code)
        {
            Write($"\u001b[{code}m");
        }

        private Task WriteSgrAnciEscapeSequenceAsync(byte code)
        {
            return WriteAsync($"\u001b[{code}m");
        }
    }
}
