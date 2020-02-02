using System;
using System.Collections.Immutable;
using System.Drawing;
using System.Runtime.CompilerServices;
using BeaverSoft.Texo.Core.Console.Decoding.Ansi;

namespace BeaverSoft.Texo.Core.Console.Rendering
{
    public class ViewBuilder : IAnsiDecoderClient
    {
        private const int MAX_BUFFER_SIZE = 2000 * 126;
        private const int INITIAL_BUFFERS_COUNT = 4;

        private static GraphicAttributes DefaultAttributes = new GraphicAttributes(Color.White, Color.Black);
        private static ViewCell DefaultCell = new ViewCell(ViewCell.EMPTY_CHARACTER, DefaultAttributes);

        private ImmutableArray<ViewCell>.Builder buffer;
        private int width, height, bufferZeroIndex, cursor, savedCursor;
        private GraphicAttributes currentAttributes;

        public ViewBuilder(int width, int height)
        {
            this.width = width;
            this.height = height;

            cursor = bufferZeroIndex = 0;
            buffer = ImmutableArray.CreateBuilder<ViewCell>(
                Math.Min(height * width * INITIAL_BUFFERS_COUNT, MAX_BUFFER_SIZE));
            currentAttributes = DefaultAttributes;
        }

        public void ChangeMode(AnsiMode mode)
        {
            // TODO: [P2] Should be implemented
        }

        public void Characters(char[] chars)
        {
            for (int i = 0; i < chars.Length; i++)
            {
                buffer[cursor++] = new ViewCell(chars[i], currentAttributes);
            }
        }

        public void ClearLine(ClearDirection direction)
        {
            throw new System.NotImplementedException();
        }

        public void ClearScreen(ClearDirection direction)
        {
            throw new System.NotImplementedException();
        }

        public void EraseCharacters(int count)
        {
            for (int i = cursor; i < cursor + count; i++)
            {
                buffer[i] = DefaultCell;
            }
        }

        public Point GetCursorPosition()
        {
            return new Point(GetRowIndex(), GetColumnIndex());
        }

        public Size GetSize()
        {
            return new Size(width, height);
        }

        public void MoveCursor(Direction direction, int amount)
        {
            throw new System.NotImplementedException();
        }

        public void MoveCursorByTabulation(ClearDirection direction, int tabs)
        {
            throw new System.NotImplementedException();
        }

        public void MoveCursorTo(Point position)
        {
            throw new System.NotImplementedException();
        }

        public void MoveCursorToBeginningOfLineAbove(int lineNumberRelativeToCurrentLine)
        {
            throw new System.NotImplementedException();
        }

        public void MoveCursorToBeginningOfLineBelow(int lineNumberRelativeToCurrentLine)
        {
            throw new System.NotImplementedException();
        }

        public void MoveCursorToColumn(int columnNumber)
        {
            throw new System.NotImplementedException();
        }

        public void RestoreCursor()
        {
            cursor = savedCursor;
        }

        public void SaveCursor()
        {
            savedCursor = cursor;
        }

        public void ScrollPageDownwards(int linesToScroll)
        {
            throw new System.NotImplementedException();
        }

        public void ScrollPageUpwards(int linesToScroll)
        {
            throw new System.NotImplementedException();
        }

        public void SetBackgroundColor(Color background)
        {
            throw new System.NotImplementedException();
        }

        public void SetForegroundColor(Color foreground)
        {
            throw new System.NotImplementedException();
        }

        public void SetGraphicRendition(GraphicRendition[] commands)
        {
            throw new System.NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetRowIndex()
        {
            return cursor / width;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetColumnIndex()
        {
            return cursor % width;
        }
    }
}
