using System.Drawing;
using StrongBeaver.Core;

namespace BeaverSoft.Texo.Core.Model.Text
{
    public interface ISpanBuilder : IFluentSyntax
    {
        Span Span { get; }

        ISpanBuilder Write(string text);

        ISpanBuilder WriteLine(string text);

        ISpanBuilder WriteLine();

        ISpanBuilder Strong(string text);

        ISpanBuilder Italic(string text);

        ISpanBuilder Strikethrough(string text);

        ISpanBuilder Marked(string text);

        ISpanBuilder Inserted(string text);

        ISpanBuilder Code(string text);

        ISpanBuilder Highlight(string text, Color highlightColor);

        ISpanBuildContext StartStrongContext();

        ISpanBuildContext StartItalicContext();

        ISpanBuildContext StartMarkedContext();

        ISpanBuildContext StartInsertedContext();

        ISpanBuildContext StartHighlightContext(Color highlightColor);

        ISpanBuilder EndContext();
    }
}