using System;
using System.Collections.Immutable;
using System.Drawing;
using System.Runtime.CompilerServices;
using BeaverSoft.Texo.Core.Console.Decoding.Ansi;
using BeaverSoft.Texo.Core.Console.Rendering.Managers;

namespace BeaverSoft.Texo.Core.Console.Rendering
{
    public class ConsoleBufferBuilder : IAnsiDecoderClient, IConsoleBufferBuilder
    {
        private const int MAX_BUFFER_SIZE = 4200 * 126;
        private const int INITIAL_BUFFERS_COUNT = 4;
        private const int CONTROL_LENGTH = 1;

        private static BufferCell DefaultCell = new BufferCell(BufferCell.EMPTY_CHARACTER, 0);

        private readonly IConsoleStylesManager styles;
        private readonly IConsoleBufferChangesManager changes;

        private ImmutableArray<BufferCell>.Builder buffer;
        private int width, widthWithControl, height;
        private int maxCapacity, screenLength, screenZeroIndex, screenEndIndex;
        private int cursor, savedCursor;
        private GraphicAttributes currentAttributes;
        private bool currentAttributesNotSaved;
        private byte currentAttributesId;

        public ConsoleBufferBuilder(int width, int height)
        {
            this.width = width;
            widthWithControl = width + CONTROL_LENGTH;
            this.height = height;

            cursor = screenZeroIndex = 0;
            screenLength = height * widthWithControl;
            screenEndIndex = screenZeroIndex + screenLength - 1;
            maxCapacity = (MAX_BUFFER_SIZE + widthWithControl) / widthWithControl * widthWithControl;
            int capacity = Math.Min(screenLength * INITIAL_BUFFERS_COUNT, maxCapacity);
            buffer = ImmutableArray.CreateBuilder<BufferCell>(capacity);
            InitialiseBuffer();

            changes = new BitArrayChangesManager(capacity, CONTROL_LENGTH);
            styles = new DefaultStylesManager(new GraphicAttributes(Color.White, Color.Black));
            currentAttributes = styles.DefaultStyle;
            currentAttributesNotSaved = false;
            currentAttributesId = 0;
        }

        public void Start()
        {
            changes.Start(screenZeroIndex, screenLength, width, cursor);
        }

        public ConsoleBuffer Snapshot(bool fullBuffer = false)
        {
            var batch = changes.Finish(screenZeroIndex, screenLength, width, cursor);

            ConsoleBuffer snapshot = new ConsoleBuffer(
                fullBuffer
                    ? buffer.ToImmutable().AsMemory()
                    : buffer.ToImmutable().AsMemory().Slice(screenZeroIndex, screenLength),
                batch,
                styles.ProvideStyles(),
                CONTROL_LENGTH);

            changes.Start(screenZeroIndex, screenLength, widthWithControl, cursor);
            return snapshot;
        }

        public void ChangeMode(AnsiMode mode)
        {
            // TODO: [P3] Should be implemented
        }

        public void Characters(char[] chars)
        {
            TryBuildStyle();

            int controlIndex = GetControlIndex();

            for (int i = 0; i < chars.Length; i++)
            {
                char character = chars[i];

                if (IsControlCharacter(character))
                {
                    if (TryProcessControlCharacter(character)) controlIndex = GetControlIndex();
                }
                else
                {
                    if (cursor == controlIndex)
                    {
                        ++cursor;
                        controlIndex = GetControlIndex();
                    }

                    buffer[cursor] = new BufferCell(character, currentAttributesId);
                    changes.AddChange(cursor++);
                }
            }
        }

        public void ClearLine(ClearDirection direction)
        {
            int startIndex, excludedEndIndex;
            TryBuildStyle();

            switch (direction)
            {
                case ClearDirection.Backward:
                    startIndex = GetFullIndexOfRow(GetRowIndex());
                    excludedEndIndex = cursor + 1;
                    break;

                case ClearDirection.Both:
                    startIndex = GetFullIndexOfRow(GetRowIndex());
                    excludedEndIndex = GetFullIndexOfRow(GetRowIndex() + 1);
                    break;

                case ClearDirection.Forward:
                default:
                    startIndex = cursor;
                    excludedEndIndex = GetFullIndexOfRow(GetRowIndex() + 1);
                    break;
            }

            for (int i = startIndex; i < excludedEndIndex; i++)
            {
                buffer[i] = new BufferCell(BufferCell.EMPTY_CHARACTER, currentAttributesId);
            }

            changes.AddChange(startIndex, excludedEndIndex - startIndex);
        }

        public void ClearScreen(ClearDirection direction)
        {
            int excludedEndIndex = screenZeroIndex + screenLength;

            for (int i = screenZeroIndex; i < excludedEndIndex; ++i)
            {
                buffer[i] = new BufferCell(BufferCell.EMPTY_CHARACTER, currentAttributesId);
            }

            changes.AddChange(screenZeroIndex, screenLength);
        }

        public void EraseCharacters(int count)
        {
            int excludedTargetEndIndex = cursor + count;
            int controlIndex = GetControlIndex();
            int excludedEndIndex = Math.Min(excludedTargetEndIndex, controlIndex);

            for (int i = cursor; i < excludedEndIndex; ++i)
            {
                buffer[i] = new BufferCell(BufferCell.EMPTY_CHARACTER, currentAttributesId);
            }

            if (excludedTargetEndIndex == controlIndex)
            {
                buffer[controlIndex] = new BufferCell(BufferCell.EMPTY_CHARACTER, currentAttributesId);
            }

            changes.AddChange(cursor, excludedEndIndex + cursor);
        }

        public Point GetCursorPosition()
        {
            int virtualScreenCursor = cursor - screenZeroIndex;
            return new Point(GetRowIndex(virtualScreenCursor), GetColumnIndex(virtualScreenCursor));
        }

        public Size GetSize()
        {
            return new Size(width, height);
        }

        public void MoveCursor(Direction direction, int amount)
        {
            switch (direction)
            {
                case Direction.Up:
                    SetCursor(cursor - (widthWithControl * amount));
                    break;

                case Direction.Down:
                    SetCursor(cursor + (widthWithControl * amount));
                    break;

                case Direction.Forward:
                    SetCursor(Math.Min(cursor + amount, GetFullIndexOfRow(GetRowIndex()) + width));
                    break;

                case Direction.Backward:
                    SetCursor(Math.Max(cursor - amount, GetFullIndexOfRow(GetRowIndex())));
                    break;
            }
        }

        public void MoveCursorByTabulation(ClearDirection direction, int tabs)
        {
            const int DEFAULT_TAB_SIZE = 8;
            int column = GetColumnIndex();
            int nextTabStop = column;

            switch (direction)
            {
                case ClearDirection.Backward:
                    nextTabStop -= DEFAULT_TAB_SIZE * tabs;
                    break;

                //case ClearDirection.Forward:
                //case ClearDirection.Both:
                default:
                    nextTabStop += DEFAULT_TAB_SIZE * tabs;
                    break;
            }

            nextTabStop = nextTabStop / DEFAULT_TAB_SIZE * DEFAULT_TAB_SIZE;
            if (nextTabStop < 0) nextTabStop = 0;
            if (nextTabStop > width) nextTabStop = width;
            SetCursor(GetFullIndexOfRow(GetRowIndex()) + nextTabStop);
        }

        public void MoveCursorTo(Point position)
        {
            SetCursor(screenZeroIndex + (position.Y * widthWithControl) + position.X);
        }

        public void MoveCursorToBeginningOfLineAbove(int lineNumberRelativeToCurrentLine)
        {
            SetCursor(GetFullIndexOfRow(GetRowIndex()) - (lineNumberRelativeToCurrentLine * widthWithControl));
        }

        public void MoveCursorToBeginningOfLineBelow(int lineNumberRelativeToCurrentLine)
        {
            SetCursor(GetFullIndexOfRow(GetRowIndex()) + (lineNumberRelativeToCurrentLine * widthWithControl));
        }

        public void MoveCursorToColumn(int columnIndex)
        {
            SetCursor(GetFullIndexOfRow(GetRowIndex()) + columnIndex);
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
            screenZeroIndex = screenZeroIndex - (linesToScroll * widthWithControl);

            if (screenZeroIndex >= 0)
            {
                screenEndIndex = screenZeroIndex + screenLength - 1;
                return;
            }

            ResizeBuffer();

            // TODO: [P1] move content
            screenZeroIndex = 0;
            screenEndIndex = screenZeroIndex + screenLength - 1;
        }

        public void ScrollPageUpwards(int linesToScroll)
        {
            screenZeroIndex = screenZeroIndex + (linesToScroll * widthWithControl);
            screenEndIndex = screenZeroIndex + screenLength - 1;

            if (screenEndIndex < buffer.Count)
            {
                return;
            }

            ResizeBuffer();

            if (screenEndIndex < buffer.Count)
            {
                return;
            }

            // TODO: [P1] Move content
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

            currentAttributesNotSaved = true;
        }

        public void MoveCursorToBeginningOfLine()
        {
            cursor = GetFullIndexOfRow(GetRowIndex());
        }

        public void MoveCursorToNextLineOrScrollPage()
        {
            int row = GetRowIndex();
            int virtualRow = row - GetRowIndex(screenZeroIndex);

            if (virtualRow >= height)
            {
                ScrollPageUpwards(virtualRow - height + 1);
                SetCursor(GetFullIndexOfRow(row));
            }
            else
            {
                SetCursor(GetFullIndexOfRow(row + 1));
            }
        }

        private void SetCursor(int newCursor)
        {
            cursor = newCursor;
            if (cursor < screenZeroIndex) cursor = screenZeroIndex;
            else if (cursor > screenEndIndex) cursor = screenEndIndex;
        }

        private void TryBuildStyle()
        {
            if (!currentAttributesNotSaved)
            {
                return;
            }

            currentAttributesId = styles.GetOrCreateStyle(currentAttributes);
        }

        private void ApplyGraphicRendition(GraphicRendition command)
        {
            switch (command)
            {
                case GraphicRendition.Reset:
                    currentAttributesId = 0;
                    currentAttributes = styles.DefaultStyle;
                    break;

                case GraphicRendition.Positive: // TODO: [P3] Not sure; oposite of GraphicRendition.Inverse
                    currentAttributes = currentAttributes
                        .SetBackground(Color.Black)
                        .SetForeground(Color.White);
                    break;

                case GraphicRendition.Inverse:
                    currentAttributes = currentAttributes.Reverse();
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

                case GraphicRendition.NoItalic:
                    currentAttributes = currentAttributes.RemoveStyle(GraphicStyle.Italic);
                    break;

                case GraphicRendition.Underline:
                case GraphicRendition.UnderlineDouble:
                    currentAttributes = currentAttributes.SetStyle(GraphicStyle.Underline);
                    break;

                case GraphicRendition.NoUnderline:
                    currentAttributes = currentAttributes.RemoveStyle(GraphicStyle.Underline);
                    break;

                case GraphicRendition.CrossedOut:
                    currentAttributes = currentAttributes.SetStyle(GraphicStyle.CrossOut);
                    break;

                case GraphicRendition.NoCrossedOut:
                    currentAttributes = currentAttributes.RemoveStyle(GraphicStyle.CrossOut);
                    break;

                case GraphicRendition.Overlined:
                    currentAttributes = currentAttributes.SetStyle(GraphicStyle.Overline);
                    break;

                case GraphicRendition.NoOverlined:
                    currentAttributes = currentAttributes.RemoveStyle(GraphicStyle.Overline);
                    break;

                case GraphicRendition.Conceal:
                    currentAttributes = currentAttributes.SetStyle(GraphicStyle.Conceal);
                    break;

                case GraphicRendition.Reveal:
                    currentAttributes = currentAttributes.RemoveStyle(GraphicStyle.Conceal);
                    break;

                case GraphicRendition.NormalIntensity:
                    currentAttributes = currentAttributes.RemoveStyle(GraphicStyle.Bold | GraphicStyle.Faint);
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
            return character < 32 || character == 127;
        }

        private bool TryProcessControlCharacter(char character)
        {
            switch (character)
            {
                case '\b': // BS: Backspace
                    MoveCursor(Direction.Backward, 1);
                    EraseCharacters(1);
                    return false;

                case '\r':
                    MoveCursorToBeginningOfLine();
                    return false;

                case '\n':
                    buffer[GetControlIndex()] = new BufferCell('\n', 0);
                    MoveCursorToNextLineOrScrollPage();
                    return true;

                default:
                    return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetRowIndex()
        {
            return cursor / widthWithControl;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetRowIndex(int specificCursor)
        {
            return specificCursor / widthWithControl;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetColumnIndex()
        {
            return cursor % widthWithControl;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetColumnIndex(int specificCursor)
        {
            return cursor % widthWithControl;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetFullIndexOfRow(int rowIndex)
        {
            return rowIndex * widthWithControl;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetControlIndex()
        {
            return (cursor / widthWithControl * widthWithControl) + width;
        }

        private void ResizeBuffer()
        {
            if (buffer.Count < maxCapacity)
            {
                return;
            }

            buffer.Capacity = Math.Min(
                buffer.Capacity + (screenLength * INITIAL_BUFFERS_COUNT),
                maxCapacity);

            InitialiseBuffer();
        }

        private void InitialiseBuffer()
        {
            for (int i = buffer.Count; i < buffer.Capacity; ++i)
            {
                buffer.Add(DefaultCell);
            }
        }
    }
}
