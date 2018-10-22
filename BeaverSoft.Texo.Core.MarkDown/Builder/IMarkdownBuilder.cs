using StrongBeaver.Core;

namespace BeaverSoft.Texo.Core.Markdown.Builder
{
    public interface IMarkdownBuilder : IFluentSyntax
    {
        IMarkdownBuilder Header(string text);

        IMarkdownBuilder Header(string text, int level);

        IMarkdownBuilder Bullet(string text);

        IMarkdownBuilder Bullet(string text, int intentLevel);

        IMarkdownBuilder Image(string path);

        IMarkdownBuilder Image(string path, string alternative);

        IMarkdownBuilder Link(string title, string path);

        IMarkdownBuilder Italic(string text);

        IMarkdownBuilder Bold(string text);

        IMarkdownBuilder CodeBlock(string language, string code);

        IMarkdownBuilder CodeInline(string code);

        IMarkdownBuilder WriteLine();

        IMarkdownBuilder WriteLine(string text);

        IMarkdownBuilder Write(string text);
    }
}
