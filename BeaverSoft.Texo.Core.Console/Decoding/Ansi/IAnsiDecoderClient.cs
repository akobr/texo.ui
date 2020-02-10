using System.Drawing;

namespace BeaverSoft.Texo.Core.Console.Decoding.Ansi
{
    public interface IAnsiDecoderClient : IDecoderClient
    {
        Point GetCursorPosition();

        Size GetSize();

        void Characters(char[] chars);

        void EraseCharacters(int count);

        void SaveCursor();

        void RestoreCursor();

        void MoveCursor(Direction direction, int amount);

        void MoveCursorToBeginningOfLineBelow(int lineNumberRelativeToCurrentLine);

        void MoveCursorToBeginningOfLineAbove(int lineNumberRelativeToCurrentLine);

        void MoveCursorToColumn(int columnIndex);

        void MoveCursorTo(Point position);

        void MoveCursorByTabulation(Direction direction, int tabs);

        void SetGraphicRendition(GraphicRendition[] commands);

        void SetBackgroundColor(Color background);

        void SetForegroundColor(Color foreground);

        void ClearScreen(ClearDirection direction);

        void ClearLine(ClearDirection direction);

        void ScrollPageUpwards(int linesToScroll);

        void ScrollPageDownwards(int linesToScroll);

        void ChangeMode(AnsiMode mode);
    }
}
