using System;
using System.Collections.Generic;
using System.Text;

namespace BeaverSoft.Texo.Core.Markdown.Builder
{
    public class MarkdownBuilder : IMarkdownBuilder
    {
        private const char NEWLINE = '\n';
        private readonly StringBuilder stringBuilder;

        public MarkdownBuilder()
        {
            stringBuilder = new StringBuilder();
        }

        private char LastCharacter
        {
            get
            {
                if (stringBuilder.Length < 1)
                {
                    return '\0';
                }

                return stringBuilder[stringBuilder.Length - 1];
            }
        }

        public IMarkdownBuilder Header(string text)
        {
            return Header(text, 1);
        }

        public IMarkdownBuilder Header(string text, int level)
        {
            stringBuilder.Append('#', Math.Max(level, 1));
            stringBuilder.Append(' ');
            stringBuilder.Append(text);
            return this;
        }

        public IMarkdownBuilder Bullet(string text)
        {
            stringBuilder.Append("* ");
            stringBuilder.Append(text);
            return this;
        }

        public IMarkdownBuilder Bullet(string text, int intentLevel)
        {
            stringBuilder.Append(' ', Math.Max(intentLevel, 0) * 3);
            stringBuilder.Append("* ");
            stringBuilder.Append(text);
            return this;
        }

        public IMarkdownBuilder Image(string path)
        {
            stringBuilder.AppendFormat("![][{0}]", path);
            return this;
        }

        public IMarkdownBuilder Image(string path, string alternative)
        {
            stringBuilder.AppendFormat("![{0}]({1})", alternative, path);
            return this;
        }

        public IMarkdownBuilder Link(string title, string path)
        {
            stringBuilder.AppendFormat("[{0}]({1})", title, path);
            return this;
        }

        public IMarkdownBuilder Italic(string text)
        {
            stringBuilder.AppendFormat("*{0}*", text);
            return this;
        }

        public IMarkdownBuilder Bold(string text)
        {
            stringBuilder.AppendFormat("**{0}**", text);
            return this;
        }

        public IMarkdownBuilder CodeBlock(string language, string code)
        {
            if (!IsOnNewLine())
            {
                stringBuilder.AppendLine();
            }

            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"```{language}");
            stringBuilder.AppendLine(code);
            stringBuilder.AppendLine("```");
            stringBuilder.AppendLine();
            return this;
        }

        public IMarkdownBuilder CodeInline(string code)
        {
            stringBuilder.AppendFormat("`{0}`", code);
            return this;
        }

        public IMarkdownBuilder Blockquotes(string quotes)
        {
            if (!IsOnNewLine())
            {
                stringBuilder.AppendLine();
            }
            
            stringBuilder.AppendLine();

            foreach (string line in GetLines(quotes))
            {
                stringBuilder.AppendFormat("> {0}", line);
                stringBuilder.AppendLine();
            }
            
            stringBuilder.AppendLine();
            return this;
        }

        public IMarkdownBuilder WriteLine()
        {
            stringBuilder.AppendLine();
            return this;
        }

        public IMarkdownBuilder WriteLine(string text)
        {
            stringBuilder.AppendLine(text);
            return this;
        }

        public IMarkdownBuilder Write(string text)
        {
            stringBuilder.Append(text);
            return this;
        }

        private bool IsOnNewLine()
        {
            return LastCharacter == NEWLINE;
        }

        private static IEnumerable<string> GetLines(string text)
        {
            return text.Split(new[] {System.Environment.NewLine}, StringSplitOptions.None);
        }
    }
}
