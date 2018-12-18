using System;
using System.Text;

namespace BeaverSoft.Texo.Core.Model.Text
{
    public class CodeBlock : ICodeBlock
    {
        public CodeBlock(IInline content)
            : this(string.Empty, content)
        {
            // no operation
        }

        public CodeBlock(string code)
            : this(string.Empty, new PlainText(code))
        {
            // no operation
        }

        public CodeBlock(string language, IInline content)
        {
            Content = content;
            Language = language;
        }

        public CodeBlock(string language, string code)
            : this(language, new PlainText(code))
        {
            // no operation
        }

        public IInline Content { get; }

        public string Language { get; }

        public string ToString(int level)
        {
            return ToString();
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("```");
            result.AppendLine(Language);
            result.Append(Content);
            result.AppendLine();
            result.Append("```");

            return result.ToString();
        }
    }
}