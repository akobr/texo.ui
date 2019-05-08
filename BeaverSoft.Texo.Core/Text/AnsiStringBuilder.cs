using System;
using System.Text;

namespace BeaverSoft.Texo.Core.Text
{
    public class AnsiStringBuilder
    {
        private readonly StringBuilder builder;

        public AnsiStringBuilder()
        {
            builder = new StringBuilder();
        }

        public AnsiStringBuilder Append(string value)
        {
            builder.Append(value);
            return this;
        }

        public AnsiStringBuilder AppendLine()
        {
            builder.AppendLine();
            return this;
        }

        public AnsiStringBuilder AppendLine(string value)
        {
            builder.AppendLine(value);
            return this;
        }

        public AnsiStringBuilder Insert(int index, string value)
        {
            builder.Insert(index, value);
            return this;
        }

        public AnsiStringBuilder AppendLink(string text, string url)
        {
            AppendSgrAnsiEscapeSequence(AnsiSgrEscapeCodes.LINK_START);
            builder.Append(text);
            builder.Append(AnsiSgrEscapeCodes.LINK_SEPARATOR);
            builder.Append(url);
            AppendSgrAnsiEscapeSequence(AnsiSgrEscapeCodes.LINK_END);
            return this;
        }

        public AnsiStringBuilder AppendBoldFormat()
        {
            return AppendSgrAnsiEscapeSequence(AnsiSgrEscapeCodes.BOLD);
        }

        public AnsiStringBuilder AppendItalicFormat()
        {
            return AppendSgrAnsiEscapeSequence(AnsiSgrEscapeCodes.ITALIC);
        }

        public AnsiStringBuilder AppendUnderlineFormat()
        {
            return AppendSgrAnsiEscapeSequence(AnsiSgrEscapeCodes.UNDERLINE);
        }

        public AnsiStringBuilder AppendCrossedOutFormat()
        {
            return AppendSgrAnsiEscapeSequence(AnsiSgrEscapeCodes.CROSSED_OUT);
        }

        public AnsiStringBuilder AppendForegroundFormat(ConsoleColor color)
        {
            string code = AnsiSgrEscapeCodes.GetForegroundColorCode(color);

            if (code == null)
            {
                return this;
            }

            return AppendSgrAnsiEscapeSequence(code);
        }

        public AnsiStringBuilder AppendForegroundFormat(int red, int green, int blue)
        {
            string code = AnsiSgrEscapeCodes.GetForegroundColorCode(red, green, blue);
            return AppendSgrAnsiEscapeSequence(code);
        }

        public AnsiStringBuilder AppendBackgroundFormat(ConsoleColor color)
        {
            string code = AnsiSgrEscapeCodes.GetBackgroundColorCode(color);

            if (code == null)
            {
                return this;
            }

            return AppendSgrAnsiEscapeSequence(code);
        }

        public AnsiStringBuilder AppendBackgroundFormat(int red, int green, int blue)
        {
            string code = AnsiSgrEscapeCodes.GetBackgroundColorCode(red, green, blue);
            return AppendSgrAnsiEscapeSequence(code);
        }

        public AnsiStringBuilder AppendFormattingReset()
        {
            return AppendSgrAnsiEscapeSequence(string.Empty);
        }

        public override string ToString()
        {
            return builder.ToString();
        }

        private AnsiStringBuilder AppendSgrAnsiEscapeSequence(string code)
        {
            builder.Append($"\u001b[{code}m");
            return this;
        }
    }
}
