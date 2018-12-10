using System;

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
    }
}