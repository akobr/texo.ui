using System;
using System.Drawing;
using System.Windows.Forms;
using BeaverSoft.Texo.Core.Console.Decoding.Ansi;

namespace VT100.Viewer.Decoding
{
    public class AnsiVisualisationDecoderClient : IAnsiDecoderClient
    {
        private readonly RichTextBox output;

        public AnsiVisualisationDecoderClient(RichTextBox output)
        {
            this.output = output;
        }

        public void ChangeMode(AnsiMode mode)
        {
            throw new NotImplementedException();
        }

        public void Characters(char[] chars)
        {
            throw new NotImplementedException();
        }

        public void ClearLine(ClearDirection direction)
        {
            throw new NotImplementedException();
        }

        public void ClearScreen(ClearDirection direction)
        {
            throw new NotImplementedException();
        }

        public void EraseCharacters(int count)
        {
            throw new NotImplementedException();
        }

        public Point GetCursorPosition()
        {
            throw new NotImplementedException();
        }

        public Size GetSize()
        {
            throw new NotImplementedException();
        }

        public void MoveCursor(Direction direction, int amount)
        {
            throw new NotImplementedException();
        }

        public void MoveCursorByTabulation(Direction direction, int tabs)
        {
            throw new NotImplementedException();
        }

        public void MoveCursorTo(Point position)
        {
            throw new NotImplementedException();
        }

        public void MoveCursorToBeginningOfLineAbove(int lineNumberRelativeToCurrentLine)
        {
            throw new NotImplementedException();
        }

        public void MoveCursorToBeginningOfLineBelow(int lineNumberRelativeToCurrentLine)
        {
            throw new NotImplementedException();
        }

        public void MoveCursorToColumn(int columnNumber)
        {
            throw new NotImplementedException();
        }

        public void RestoreCursor()
        {
            throw new NotImplementedException();
        }

        public void SaveCursor()
        {
            throw new NotImplementedException();
        }

        public void ScrollPageDownwards(int linesToScroll)
        {
            throw new NotImplementedException();
        }

        public void ScrollPageUpwards(int linesToScroll)
        {
            throw new NotImplementedException();
        }

        public void SetBackgroundColor(Color background)
        {
            throw new NotImplementedException();
        }

        public void SetForegroundColor(Color foreground)
        {
            throw new NotImplementedException();
        }

        public void SetGraphicRendition(GraphicRendition[] commands)
        {
            throw new NotImplementedException();
        }
    }
}
