using System;
using System.Collections.Immutable;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;
using BeaverSoft.Texo.Core.Console.Decoding.Ansi;

namespace BeaverSoft.Texo.Core.Console.Rendering
{
    public class ViewBuilder : IAnsiDecoderClient
    {
        private const int MAX_BUFFER_SIZE = 2000 * 126;
        private const int INITIAL_BUFFERS_COUNT = 4;

        private static ViewCell DefaultCell = new ViewCell(ViewCell.EMPTY_CHARACTER, 0);

        private readonly ViewStyles styles;
        private readonly ViewChanges changes;

        private ImmutableArray<ViewCell>.Builder buffer;
        private int width, height, bufferZeroIndex, cursor, savedCursor;
        private GraphicAttributes currentAttributes;
        private bool currentAttributesNotSaved;
        private byte currentAttributesId;

        public ViewBuilder(int width, int height)
        {
            this.width = width;
            this.height = height;

            cursor = bufferZeroIndex = 0;
            buffer = ImmutableArray.CreateBuilder<ViewCell>(
                Math.Min(height * width * INITIAL_BUFFERS_COUNT, MAX_BUFFER_SIZE));

            changes = new ViewChanges();
            styles = new ViewStyles(new GraphicAttributes(Color.White, Color.Black));
            currentAttributes = styles.DefaultStyle;
            currentAttributesNotSaved = false;
            currentAttributesId = 0;
        }

        public void ChangeMode(AnsiMode mode)
        {
            // TODO: [P3] Should be implemented
        }

        public void Characters(char[] chars)
        {
            StringBuilder writeBuilder = new StringBuilder(chars.Length);

            foreach (char character in chars)
            {
                if (IsControlCharacter(character))
                {
                    WriteText(writeBuilder.ToString());
                    writeBuilder.Clear();
                    TryProcessControlCharacter(character);
                }
                else
                {
                    writeBuilder.Append(character);
                }
            }

            if (writeBuilder.Length < 1)
            {
                return;
            }

            WriteText(writeBuilder.ToString());

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
            currentAttributes = currentAttributes.SetBackground(background);
            currentAttributesNotSaved = true;
        }

        public void SetForegroundColor(Color foreground)
        {
            currentAttributes = currentAttributes.SetForeground(foreground);
            currentAttributesNotSaved = true;
        }

        public void SetGraphicRendition(GraphicRendition[] commands)
        {
            foreach (GraphicRendition command in commands)
            {
                ApplyGraphicRendition(command);
            }

            // TODO: Maybe save here?
            currentAttributesNotSaved = true;
        }

        public void MoveCursorToBeginningOfLine()
        {
            cursor = GetFullIndexOfRow(GetRowIndex());
        }

        public void MoveCursorToNextLineOrScrollPage()
        {
            int row = GetRowIndex();
            if (row - GetRowIndex(bufferZeroIndex) >= height)
            {
                ScrollPageUpwards(1);
                cursor = GetFullIndexOfRow(row);
            }
            else
            {
                cursor = GetFullIndexOfRow(row + 1);
            }
        }

        private void ApplyGraphicRendition(GraphicRendition command)
        {
            switch (command)
            {
                case GraphicRendition.Reset:
                    currentAttributesId = 0;
                    currentAttributes = styles.DefaultStyle;
                    break;

                case GraphicRendition.Positive: // TODO: [P3] Not sure
                    currentAttributes = currentAttributes
                        .SetBackground(Color.Black)
                        .SetForeground(Color.White);
                    break;

                case GraphicRendition.Bold:
                    currentAttributes = currentAttributes.SetStyle(GraphicStyle.Bold);
                    break;

                case GraphicRendition.Faint:
                    currentAttributes = currentAttributes.SetStyle(GraphicStyle.Faint);
                    break;

                case GraphicRendition.Italic:
                    currentAttributes = currentAttributes.SetStyle(GraphicStyle.Italic);
                    break;

                case GraphicRendition.Underline:
                case GraphicRendition.UnderlineDouble:
                    currentAttributes = currentAttributes.SetStyle(GraphicStyle.Underline);
                    break;

                case GraphicRendition.Inverse:
                    currentAttributes = currentAttributes.Reverse();
                    break;

                case GraphicRendition.Conceal:
                    currentAttributes = currentAttributes.SetStyle(GraphicStyle.Conceal);
                    break;

                case GraphicRendition.NormalIntensity:
                    currentAttributes = currentAttributes.RemoveStyle(GraphicStyle.Bold | GraphicStyle.Faint);
                    break;

                case GraphicRendition.NoUnderline:
                    currentAttributes = currentAttributes.RemoveStyle(GraphicStyle.Underline);
                    break;

                case GraphicRendition.Reveal:
                    currentAttributes = currentAttributes.RemoveStyle(GraphicStyle.Conceal);
                    break;

                case GraphicRendition.ForegroundNormalBlack:
                    currentAttributes = currentAttributes.SetForeground(Color.FromArgb(0, 0, 0));
                    break;

                case GraphicRendition.ForegroundNormalRed:
                    currentAttributes = currentAttributes.SetForeground(Color.FromArgb(194, 54, 33));
                    break;

                case GraphicRendition.ForegroundNormalGreen:
                    currentAttributes = currentAttributes.SetForeground(Color.FromArgb(37, 188, 36));
                    break;

                case GraphicRendition.ForegroundNormalYellow:
                    currentAttributes = currentAttributes.SetForeground(Color.FromArgb(173, 173, 39));
                    break;

                case GraphicRendition.ForegroundNormalBlue:
                    currentAttributes = currentAttributes.SetForeground(Color.FromArgb(73, 46, 225));
                    break;

                case GraphicRendition.ForegroundNormalMagenta:
                    currentAttributes = currentAttributes.SetForeground(Color.FromArgb(211, 56, 211));
                    break;

                case GraphicRendition.ForegroundNormalCyan:
                    currentAttributes = currentAttributes.SetForeground(Color.FromArgb(51, 187, 200));
                    break;

                case GraphicRendition.ForegroundNormalWhite:
                    currentAttributes = currentAttributes.SetForeground(Color.FromArgb(203, 204, 205));
                    break;

                case GraphicRendition.ForegroundNormalReset:
                case GraphicRendition.ForegroundBrightReset:
                    currentAttributes = currentAttributes.SetForeground(styles.DefaultStyle.Foreground);
                    break;

                case GraphicRendition.BackgroundNormalBlack:
                    currentAttributes = currentAttributes.SetBackground(Color.FromArgb(0, 0, 0));
                    break;

                case GraphicRendition.BackgroundNormalRed:
                    currentAttributes = currentAttributes.SetBackground(Color.FromArgb(194, 54, 33));
                    break;

                case GraphicRendition.BackgroundNormalGreen:
                    currentAttributes = currentAttributes.SetBackground(Color.FromArgb(37, 188, 36));
                    break;

                case GraphicRendition.BackgroundNormalYellow:
                    currentAttributes = currentAttributes.SetBackground(Color.FromArgb(173, 173, 39));
                    break;

                case GraphicRendition.BackgroundNormalBlue:
                    currentAttributes = currentAttributes.SetBackground(Color.FromArgb(73, 46, 225));
                    break;

                case GraphicRendition.BackgroundNormalMagenta:
                    currentAttributes = currentAttributes.SetBackground(Color.FromArgb(211, 56, 211));
                    break;

                case GraphicRendition.BackgroundNormalCyan:
                    currentAttributes = currentAttributes.SetBackground(Color.FromArgb(51, 187, 200));
                    break;

                case GraphicRendition.BackgroundNormalWhite:
                    currentAttributes = currentAttributes.SetBackground(Color.FromArgb(203, 204, 205));
                    break;

                case GraphicRendition.BackgroundNormalReset:
                case GraphicRendition.BackgroundBrightReset:
                    currentAttributes = currentAttributes.SetBackground(styles.DefaultStyle.Background);
                    break;

                case GraphicRendition.ForegroundBrightBlack:
                    currentAttributes = currentAttributes.SetForeground(Color.FromArgb(129, 131, 131));
                    break;

                case GraphicRendition.ForegroundBrightRed:
                    currentAttributes = currentAttributes.SetForeground(Color.FromArgb(252, 57, 31));
                    break;

                case GraphicRendition.ForegroundBrightGreen:
                    currentAttributes = currentAttributes.SetForeground(Color.FromArgb(49, 231, 34));
                    break;

                case GraphicRendition.ForegroundBrightYellow:
                    currentAttributes = currentAttributes.SetForeground(Color.FromArgb(234, 236, 35));
                    break;

                case GraphicRendition.ForegroundBrightBlue:
                    currentAttributes = currentAttributes.SetForeground(Color.FromArgb(88, 51, 255));
                    break;

                case GraphicRendition.ForegroundBrightMagenta:
                    currentAttributes = currentAttributes.SetForeground(Color.FromArgb(249, 53, 248));
                    break;

                case GraphicRendition.ForegroundBrightCyan:
                    currentAttributes = currentAttributes.SetForeground(Color.FromArgb(20, 240, 240));
                    break;

                case GraphicRendition.ForegroundBrightWhite:
                    currentAttributes = currentAttributes.SetForeground(Color.FromArgb(233, 235, 235));
                    break;

                case GraphicRendition.BackgroundBrightBlack:
                    currentAttributes = currentAttributes.SetBackground(Color.FromArgb(129, 131, 131));
                    break;

                case GraphicRendition.BackgroundBrightRed:
                    currentAttributes = currentAttributes.SetBackground(Color.FromArgb(252, 57, 31));
                    break;

                case GraphicRendition.BackgroundBrightGreen:
                    currentAttributes = currentAttributes.SetBackground(Color.FromArgb(49, 231, 34));
                    break;

                case GraphicRendition.BackgroundBrightYellow:
                    currentAttributes = currentAttributes.SetBackground(Color.FromArgb(234, 236, 35));
                    break;

                case GraphicRendition.BackgroundBrightBlue:
                    currentAttributes = currentAttributes.SetBackground(Color.FromArgb(88, 51, 255));
                    break;

                case GraphicRendition.BackgroundBrightMagenta:
                    currentAttributes = currentAttributes.SetBackground(Color.FromArgb(249, 53, 248));
                    break;

                case GraphicRendition.BackgroundBrightCyan:
                    currentAttributes = currentAttributes.SetBackground(Color.FromArgb(20, 240, 240));
                    break;

                case GraphicRendition.BackgroundBrightWhite:
                    currentAttributes = currentAttributes.SetBackground(Color.FromArgb(233, 235, 235));
                    break;

                case GraphicRendition.BlinkSlow:
                case GraphicRendition.BlinkRapid:
                    currentAttributes = currentAttributes.SetStyle(GraphicStyle.Blink);
                    break;

                case GraphicRendition.NoBlink:
                    currentAttributes = currentAttributes.RemoveStyle(GraphicStyle.Blink);
                    break;

                case GraphicRendition.DefaultFont:
                default:
                    // Not supported
                    break;
            }
        }

        private bool IsControlCharacter(char character)
        {
            return character == '\r' || character == '\n';
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetRowIndex()
        {
            return cursor / width;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetRowIndex(int specificCursor)
        {
            return specificCursor / width;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetColumnIndex()
        {
            return cursor % width;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetFullIndexOfRow(int rowIndex)
        {
            return rowIndex * width;
        }
    }
}
