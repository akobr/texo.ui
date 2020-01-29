using System;
using System.Drawing;
using System.Windows.Forms;
using BeaverSoft.Texo.Core.Console.Decoding.Ansi;

namespace VT100.Viewer.Decoding
{
    public class AnsiDecoderClient : IAnsiDecoderClient
    {
        private const int DEFAULT_TAB_SIZE = 8;

        private readonly RichTextBox output;
        private int width, height;
        private SelectionRendition rendition;
        private Point? savedCursor;

        public AnsiDecoderClient(RichTextBox output, int width, int height)
        {
            this.output = output;
            this.width = width;
            this.height = height;

            rendition = new SelectionRendition();
            ClearBuffer();
        }

        private void ClearBuffer()
        {
            string[] lines = new string[height];

            for (int i = 0; i < height; i++)
            {
                lines[i] = new string(' ', width);
            }

            output.Clear();
            output.Lines = lines;
            output.SelectAll();
            SetDefaultRendition();
        }

        private void SetRendition()
        {
            output.BackColor = rendition.BackColor;
            output.SelectionColor = rendition.Color;
            output.SelectionFont = rendition.Font;
        }

        private void SetDefaultRendition()
        {
            output.SelectionFont = SelectionRendition.DEFAULT_FONT;
            output.SelectionBackColor = SelectionRendition.DEFAULT_BACK_COLOR;
            output.SelectionColor = SelectionRendition.DEFAULT_COLOR;
        }

        public void ChangeMode(AnsiMode mode)
        {

        }

        public void Characters(char[] chars)
        {
            if (chars.Length == 1
                && TryProcessControlCharacter(chars[0]))
            {
                return;
            }

            int index = output.SelectionStart;
            output.Select(index, chars.Length);
            SetRendition();
            output.SelectedText = new string(chars);
            output.Select(index + chars.Length, 0);
        }

        public void EraseCharacters(int count)
        {
            int index = output.SelectionStart;
            int line = output.GetLineFromCharIndex(index);
            int firstLineIndex = output.GetFirstCharIndexFromLine(line);
            int lastLineIndex = firstLineIndex + width - 1;

            int endIndex = index + count;
            if (endIndex > lastLineIndex) endIndex = lastLineIndex;
            int finalCount = endIndex - index;

            output.Select(index, finalCount);
            SetDefaultRendition();
            output.SelectedText = new string(' ', finalCount);
            output.Select(index, 0);
        }

        private bool TryProcessControlCharacter(char character)
        {
            switch (character)
            {
                case '\a':   // BEL
                case '\x1B': // ESC
                    return true;

                case '\r':
                    MoveCursorToBeginningOfLine();
                    return true;

                case '\n':
                    MoveCursorToNextLineOrScrollPage();
                    return true;

                default:
                    return false;
            }
        }

        public void ClearLine(ClearDirection direction)
        {
            int clearSizeLength = 0;
            int index = output.SelectionStart;
            int line = output.GetLineFromCharIndex(index);
            int firstLineIndex = output.GetFirstCharIndexFromLine(line);

            switch (direction)
            {
                case ClearDirection.Forward:
                    clearSizeLength = width - (index - firstLineIndex);
                    output.Select(firstLineIndex, clearSizeLength);
                    break;

                case ClearDirection.Backward:
                    clearSizeLength = index - firstLineIndex;
                    output.Select(index, clearSizeLength);
                    break;

                case ClearDirection.Both:
                    clearSizeLength = width;
                    output.Select(firstLineIndex, clearSizeLength);
                    break;
            }

            SetDefaultRendition();
            string clearText = new string(' ', clearSizeLength);
            output.SelectedText = clearText;
            output.Select(index, 0);
        }

        public void ClearScreen(ClearDirection direction)
        {
            // TODO: [P2] use directions
            ClearBuffer();
        }

        public Point GetCursorPosition()
        {
            int index = output.SelectionStart;
            int line = output.GetLineFromCharIndex(index);
            int firstLineIndex = output.GetFirstCharIndexFromLine(line);
            int column = index - firstLineIndex;

            return new Point(column, line);
        }

        public Size GetSize()
        {
            return new Size(width, height);
        }

        public void MoveCursor(Direction direction, int amount)
        {
            int index = output.SelectionStart;

            switch (direction)
            {
                case Direction.Up:
                    MoveCursorUp(amount, index);
                    break;

                case Direction.Down:
                    MoveCursorDown(amount, index);
                    break;

                case Direction.Forward:
                    index += amount;
                    output.Select(index, 0);
                    break;

                case Direction.Backward:
                    index -= amount;
                    output.Select(index, 0);
                    break;
            }
        }

        private void MoveCursorDown(int amount, int index)
        {
            int line = output.GetLineFromCharIndex(index);
            int firstLineIndex = output.GetFirstCharIndexFromLine(line);
            int column = index - firstLineIndex;

            // get target line
            int targetLine = line + amount;
            if (targetLine >= output.Lines.Length) targetLine = output.Lines.Length - 1;

            // calculate and set cursor
            int targetIndex = output.GetFirstCharIndexFromLine(targetLine) + column;
            output.Select(targetIndex, 0);
        }

        private void MoveCursorUp(int amount, int index)
        {
            int line = output.GetLineFromCharIndex(index);
            int firstLineIndex = output.GetFirstCharIndexFromLine(line);
            int column = index - firstLineIndex;

            // get target line
            int targetLine = line - amount;
            if (targetLine < 0) targetLine = 0;

            // calculate and set cursor
            int targetIndex = output.GetFirstCharIndexFromLine(targetLine) + column;
            output.Select(targetIndex, 0);
        }

        public void MoveCursorTo(Point position)
        {
            int index = output.GetFirstCharIndexFromLine(position.Y);
            output.Select(index + position.X, 0);
        }

        public void MoveCursorToBeginningOfLine()
        {
            int index = output.SelectionStart;
            int line = output.GetLineFromCharIndex(index);
            int targetIndex = output.GetFirstCharIndexFromLine(line);
            output.Select(targetIndex, 0);
        }

        public void MoveCursorToNextLineOrScrollPage()
        {
            int index = output.SelectionStart;
            int line = output.GetLineFromCharIndex(index);
            if (line + 1 == output.Lines.Length)
            {
                ScrollPageUpwards(1);
                index = output.GetFirstCharIndexFromLine(line);
            }
            else
            {
                index = output.GetFirstCharIndexFromLine(line + 1);
            }
            output.Select(index, 0);
        }

        public void MoveCursorToBeginningOfLineAbove(int lineNumberRelativeToCurrentLine)
        {
            int index = output.SelectionStart;
            int line = output.GetLineFromCharIndex(index);
            int targetLine = line - lineNumberRelativeToCurrentLine;
            if (targetLine < 0) targetLine = 0;
            int targetIndex = output.GetFirstCharIndexFromLine(targetLine);
            output.Select(targetIndex, 0);
        }

        public void MoveCursorToBeginningOfLineBelow(int lineNumberRelativeToCurrentLine)
        {
            int index = output.SelectionStart;
            int line = output.GetLineFromCharIndex(index);
            int targetLine = line + lineNumberRelativeToCurrentLine;
            if (targetLine >= output.Lines.Length) targetLine = output.Lines.Length - 1;
            int targetIndex = output.GetFirstCharIndexFromLine(targetLine);
            output.Select(targetIndex, 0);
        }

        public void MoveCursorToColumn(int columnNumber)
        {
            int index = output.SelectionStart;
            int line = output.GetLineFromCharIndex(index);
            int firstLineIndex = output.GetFirstCharIndexFromLine(line);
            output.Select(firstLineIndex + columnNumber - 1, 0);
        }

        public void MoveCursorByTabulation(ClearDirection direction, int tabs)
        {
            int column = output.SelectionStart;
            int nextTabStop = column;

            switch (direction)
            {
                case ClearDirection.Backward:
                    nextTabStop -= DEFAULT_TAB_SIZE;
                    break;

                //case ClearDirection.Forward:
                //case ClearDirection.Both:
                default:
                    nextTabStop += DEFAULT_TAB_SIZE;
                    break;
            }

            nextTabStop = nextTabStop / DEFAULT_TAB_SIZE * DEFAULT_TAB_SIZE;
            if (nextTabStop < 0) nextTabStop = 0;
            if (nextTabStop >= width) nextTabStop = width - 1;
            output.Select(nextTabStop, 0);
        }

        public void RestoreCursor()
        {
            if (!savedCursor.HasValue)
            {
                return;
            }

            MoveCursorTo(savedCursor.Value);
            savedCursor = null;
        }

        public void SaveCursor()
        {
            savedCursor = GetCursorPosition();
        }

        public void ScrollPageDownwards(int linesToScroll)
        {
            if (linesToScroll >= height)
            {
                ClearBuffer();
                return;
            }

            string[] lines = output.Lines;

            for (int i = lines.Length - linesToScroll - 1; i >= 0; i--)
            {
                lines[i + linesToScroll] = lines[i];
            }

            for (int i = 0; i < linesToScroll; i++)
            {
                lines[i] = new string(' ', width);
            }

            output.Lines = lines;
        }

        public void ScrollPageUpwards(int linesToScroll)
        {
            if (linesToScroll >= height)
            {
                ClearBuffer();
                return;
            }

            string[] lines = output.Lines;

            for (int i = linesToScroll; i < lines.Length; i++)
            {
                lines[i - linesToScroll] = lines[i];
            }

            for (int i = lines.Length - linesToScroll; i < lines.Length; i++)
            {
                lines[i] = new string(' ', width);
            }

            output.Lines = lines;
        }

        public void SetBackgroundColor(Color background)
        {
            rendition.BackColor = background;
        }

        public void SetForegroundColor(Color foreground)
        {
            rendition.Color = foreground;
        }

        public void SetGraphicRendition(GraphicRendition[] commands)
        {
            foreach (GraphicRendition command in commands)
            {
                ApplyGraphicRendition(command);
            }
        }

        private void ApplyGraphicRendition(GraphicRendition command)
        {
            switch (command)
            {
                case GraphicRendition.Reset:
                    rendition.Reset();
                    break;

                case GraphicRendition.Positive: // Not sure
                    rendition.BackColor = Color.Black;
                    rendition.Color = Color.White;
                    break;

                case GraphicRendition.Bold:
                    rendition.AddFontStyle(FontStyle.Bold);
                    break;

                case GraphicRendition.Faint:
                    rendition.RemoveFontStyle(FontStyle.Bold);
                    break;

                case GraphicRendition.Italic:
                    rendition.AddFontStyle(FontStyle.Italic);
                    break;

                case GraphicRendition.Underline:
                    rendition.AddFontStyle(FontStyle.Underline);
                    break;

                case GraphicRendition.Inverse:
                    var temp = rendition.BackColor;
                    rendition.BackColor = rendition.Color;
                    rendition.Color = temp;
                    break;

                case GraphicRendition.Conceal:
                    rendition.Conceal();
                    break;

                case GraphicRendition.NormalIntensity:
                    rendition.Font = new Font(output.Font, FontStyle.Regular);
                    break;

                case GraphicRendition.NoUnderline:
                    rendition.RemoveFontStyle(FontStyle.Underline);
                    break;

                case GraphicRendition.Reveal:
                    rendition.Reveal();
                    break;

                case GraphicRendition.ForegroundNormalBlack:
                    SetForegroundColor(Color.FromArgb(0, 0, 0));
                    break;

                case GraphicRendition.ForegroundNormalRed:
                    SetForegroundColor(Color.FromArgb(194, 54, 33));
                    break;

                case GraphicRendition.ForegroundNormalGreen:
                    SetForegroundColor(Color.FromArgb(37, 188, 36));
                    break;

                case GraphicRendition.ForegroundNormalYellow:
                    SetForegroundColor(Color.FromArgb(173, 173, 39));
                    break;

                case GraphicRendition.ForegroundNormalBlue:
                    SetForegroundColor(Color.FromArgb(73, 46, 225));
                    break;

                case GraphicRendition.ForegroundNormalMagenta:
                    SetForegroundColor(Color.FromArgb(211, 56, 211));
                    break;

                case GraphicRendition.ForegroundNormalCyan:
                    SetForegroundColor(Color.FromArgb(51, 187, 200));
                    break;

                case GraphicRendition.ForegroundNormalWhite:
                    SetForegroundColor(Color.FromArgb(203, 204, 205));
                    break;

                case GraphicRendition.ForegroundNormalReset:
                    SetForegroundColor(SelectionRendition.DEFAULT_COLOR);
                    break;

                case GraphicRendition.BackgroundNormalBlack:
                    SetBackgroundColor(Color.FromArgb(0, 0, 0));
                    break;

                case GraphicRendition.BackgroundNormalRed:
                    SetBackgroundColor(Color.FromArgb(194, 54, 33));
                    break;

                case GraphicRendition.BackgroundNormalGreen:
                    SetBackgroundColor(Color.FromArgb(37, 188, 36));
                    break;

                case GraphicRendition.BackgroundNormalYellow:
                    SetBackgroundColor(Color.FromArgb(173, 173, 39));
                    break;

                case GraphicRendition.BackgroundNormalBlue:
                    SetBackgroundColor(Color.FromArgb(73, 46, 225));
                    break;

                case GraphicRendition.BackgroundNormalMagenta:
                    SetBackgroundColor(Color.FromArgb(211, 56, 211));
                    break;

                case GraphicRendition.BackgroundNormalCyan:
                    SetBackgroundColor(Color.FromArgb(51, 187, 200));
                    break;

                case GraphicRendition.BackgroundNormalWhite:
                    SetBackgroundColor(Color.FromArgb(203, 204, 205));
                    break;

                case GraphicRendition.BackgroundNormalReset:
                    SetBackgroundColor(SelectionRendition.DEFAULT_BACK_COLOR);
                    break;

                case GraphicRendition.ForegroundBrightBlack:
                    SetForegroundColor(Color.FromArgb(129, 131, 131));
                    break;

                case GraphicRendition.ForegroundBrightRed:
                    SetForegroundColor(Color.FromArgb(252, 57, 31));
                    break;

                case GraphicRendition.ForegroundBrightGreen:
                    SetForegroundColor(Color.FromArgb(49, 231, 34));
                    break;

                case GraphicRendition.ForegroundBrightYellow:
                    SetForegroundColor(Color.FromArgb(234, 236, 35));
                    break;

                case GraphicRendition.ForegroundBrightBlue:
                    SetForegroundColor(Color.FromArgb(88, 51, 255));
                    break;

                case GraphicRendition.ForegroundBrightMagenta:
                    SetForegroundColor(Color.FromArgb(249, 53, 248));
                    break;

                case GraphicRendition.ForegroundBrightCyan:
                    SetForegroundColor(Color.FromArgb(20, 240, 240));
                    break;

                case GraphicRendition.ForegroundBrightWhite:
                    SetForegroundColor(Color.FromArgb(233, 235, 235));
                    break;

                case GraphicRendition.ForegroundBrightReset:
                    SetForegroundColor(SelectionRendition.DEFAULT_COLOR);
                    break;

                case GraphicRendition.BackgroundBrightBlack:
                    SetBackgroundColor(Color.FromArgb(129, 131, 131));
                    break;

                case GraphicRendition.BackgroundBrightRed:
                    SetBackgroundColor(Color.FromArgb(252, 57, 31));
                    break;

                case GraphicRendition.BackgroundBrightGreen:
                    SetBackgroundColor(Color.FromArgb(49, 231, 34));
                    break;

                case GraphicRendition.BackgroundBrightYellow:
                    SetBackgroundColor(Color.FromArgb(234, 236, 35));
                    break;

                case GraphicRendition.BackgroundBrightBlue:
                    SetBackgroundColor(Color.FromArgb(88, 51, 255));
                    break;

                case GraphicRendition.BackgroundBrightMagenta:
                    SetBackgroundColor(Color.FromArgb(249, 53, 248));
                    break;

                case GraphicRendition.BackgroundBrightCyan:
                    SetBackgroundColor(Color.FromArgb(20, 240, 240));
                    break;

                case GraphicRendition.BackgroundBrightWhite:
                    SetBackgroundColor(Color.FromArgb(233, 235, 235));
                    break;

                case GraphicRendition.BackgroundBrightReset:
                    SetBackgroundColor(SelectionRendition.DEFAULT_BACK_COLOR);
                    break;

                case GraphicRendition.DefaultFont:
                case GraphicRendition.BlinkSlow:
                case GraphicRendition.BlinkRapid:
                case GraphicRendition.NoBlink:
                case GraphicRendition.UnderlineDouble:
                default:
                    // Not supported
                    break;
            }
        }

        private class SelectionRendition
        {
            public static Font DEFAULT_FONT = new Font("Consolas", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            public static Color DEFAULT_BACK_COLOR = Color.Black;
            public static Color DEFAULT_COLOR = Color.FromArgb(203, 204, 205);

            private Color ConcealColor;

            public Color BackColor;
            public Color Color;
            public Font Font;

            public SelectionRendition()
            {
                Reset();
            }

            public void AddFontStyle(FontStyle style)
            {
                Font = new Font(Font, Font.Style | style);
            }

            public void RemoveFontStyle(FontStyle style)
            {
                Font = new Font(Font, Font.Style & ~style);
            }

            public void Conceal()
            {
                ConcealColor = Color;
                Color = BackColor;
            }

            public void Reveal()
            {
                Color = ConcealColor;
            }

            public void Reset()
            {
                Font = DEFAULT_FONT;
                BackColor = DEFAULT_BACK_COLOR;
                Color = DEFAULT_COLOR;
                ConcealColor = DEFAULT_COLOR;
            }
        }
    }
}
