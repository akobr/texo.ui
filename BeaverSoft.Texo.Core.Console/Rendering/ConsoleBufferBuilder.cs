using System;
using System.Buffers;
using System.Drawing;
using System.Runtime.CompilerServices;
using BeaverSoft.Texo.Core.Console.Decoding.Ansi;
using BeaverSoft.Texo.Core.Console.Rendering.Managers;

namespace BeaverSoft.Texo.Core.Console.Rendering
{
    // TODO: [P2] check everything agains https://terminalguide.netlify.com/seq/
    public class ConsoleBufferBuilder : IAnsiDecoderClient, IConsoleBufferBuilder
    {
        private const int MAX_BUFFER_SIZE = 4200 * 126;
        private const int INITIAL_BUFFERS_COUNT = 4;

        private readonly ArrayPool<BufferCell> buffersPool;
        private readonly IConsoleStylesManager styles;
        private readonly IConsoleBufferChangesManager changes;

        private BufferCell[] buffer;
        private int bufferSize;
        private int width, widthWithControl, height;
        private int screenLength, screenZeroIndex, screenEndIndex;
        private int maxCapacity, capacityIncrement;
        private int cursor, savedCursor;

        private GraphicAttributes currentAttributes;
        private bool currentAttributesNotSaved;
        private byte currentAttributesId;

        public ConsoleBufferBuilder(int width, int heigh)
            : this(width, heigh, ArrayPool<BufferCell>.Shared)
        {
            // no operation
        }

        public ConsoleBufferBuilder(int width, int height, ArrayPool<BufferCell> buffersPool)
        {
            this.buffersPool = buffersPool;
            cursor = screenZeroIndex = 0;
            SetScreenSize(width, height);

            bufferSize = Math.Min(capacityIncrement, maxCapacity);
            buffer = buffersPool.Rent(bufferSize);
            changes = new BitArrayChangesManager(capacityIncrement);
            styles = new DefaultStylesManager(new GraphicAttributes(Color.White, Color.Black));

            currentAttributes = styles.DefaultStyle;
            currentAttributesNotSaved = false;
            currentAttributesId = 0;
        }

        public void Dispose()
        {
            buffersPool.Return(buffer);
        }

        public void Start()
        {
            changes.Start(GetCurrectFlatScreen(), cursor);
        }

        public ConsoleBuffer Snapshot(ConsoleBufferType type = ConsoleBufferType.Screen)
        {
            SizedBufferSequence screen = GetCurrectFlatScreen();
            BufferCell[] snapshotBuffer = BuildBufferSnapshot(type, out int snapshotStartIndex, out int snapshotLength);
            ConsoleBufferChangeBatch batch = changes.Finish(screen, cursor, snapshotStartIndex, snapshotLength);
            ConsoleBuffer snapshot = new ConsoleBuffer(
                type,
                snapshotBuffer,
                batch,
                styles.ProvideStyles(),
                buffersPool);

            changes.Start(screen, cursor);
            return snapshot;
        }

        public Rectangle GetCurrectScreen()
        {
            return new Rectangle(0, GetRowIndex(screenZeroIndex), width, height);
        }

        public SizedBufferSequence GetCurrectFlatScreen()
        {
            return new SizedBufferSequence(screenZeroIndex, screenEndIndex, width);
        }

        private BufferCell[] BuildBufferSnapshot(ConsoleBufferType type, out int startIndex, out int length)
        {
            switch (type)
            {
                case ConsoleBufferType.AllChanges:
                    startIndex = Math.Min(GetFullIndexOfRow(GetRowIndex(changes.AllChangeSequence.StartIndex)), screenZeroIndex);
                    int endIndex = Math.Max(GetFullIndexOfRow(GetRowIndex(changes.AllChangeSequence.EndIndex)) + widthWithControl - 1, screenEndIndex);
                    length = endIndex - startIndex + 1;
                    BufferCell[] allChanges = buffersPool.Rent(length);
                    Array.Copy(buffer, startIndex, allChanges, 0, length);
                    return allChanges;

                case ConsoleBufferType.Full:
                    BufferCell[] full = buffersPool.Rent(bufferSize);
                    Array.Copy(buffer, 0, full, 0, bufferSize);
                    startIndex = 0;
                    length = bufferSize;
                    return full;

                // case ConsoleBufferType.Screen:
                default:
                    BufferCell[] screen = buffersPool.Rent(screenLength);
                    Array.Copy(buffer, screenZeroIndex, screen, 0, screenLength);
                    startIndex = screenZeroIndex;
                    length = screenLength;
                    return screen;
            }
        }

        public void ChangeMode(AnsiMode mode)
        {
            // TODO: [P3] Should be implemented
        }

        public void Characters(char[] chars)
        {
            TryBuildStyle();

            int controlIndex = GetControlIndex();

            for (int i = 0; i < chars.Length; ++i)
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
                        buffer[cursor] = new BufferCell((char)6, 0);
                        MoveCursorToBeginningOfLineBelow(1);
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

            for (int i = startIndex; i < excludedEndIndex; ++i)
            {
                buffer[i] = new BufferCell(BufferCell.EMPTY_CHARACTER, currentAttributesId);
            }

            changes.AddChange(startIndex, excludedEndIndex - startIndex);
        }

        public void ClearScreen(ClearDirection direction)
        {
            int startClearIndex, endClearIndex;
            TryBuildStyle();

            switch (direction)
            {
                case ClearDirection.Backward:
                    startClearIndex = screenZeroIndex;
                    endClearIndex = cursor;
                    break;

                case ClearDirection.Both:
                    startClearIndex = screenZeroIndex;
                    endClearIndex = screenEndIndex;
                    break;

                // case ClearDirection.Forward:
                default:
                    startClearIndex = cursor;
                    endClearIndex = screenEndIndex;
                    break;
            }

            for (int i = startClearIndex; i <= endClearIndex; ++i)
            {
                buffer[i] = new BufferCell(BufferCell.EMPTY_CHARACTER, currentAttributesId);
            }

            changes.AddChange(startClearIndex, endClearIndex - startClearIndex + 1);
        }

        public void EraseCharacters(int count)
        {
            TryBuildStyle();

            if (count < 1) count = 1;
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

            changes.AddChange(cursor, excludedEndIndex - cursor);
        }

        public Point GetCursorPosition()
        {
            int virtualScreenCursor = cursor - screenZeroIndex;
            return new Point(GetRowIndex(virtualScreenCursor) + 1, GetColumnIndex(virtualScreenCursor) + 1);
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
                    int requestedIndexForward = cursor + amount;
                    int maxIndexForward = GetFullIndexOfRow(GetRowIndex()) + width;
                    SetCursor(Math.Min(requestedIndexForward, maxIndexForward));
                    break;

                case Direction.Backward:
                    int requestedIndexBackward = cursor - amount;
                    int maxIndexBackward = GetFullIndexOfRow(GetRowIndex());
                    SetCursor(Math.Max(requestedIndexBackward, maxIndexBackward));
                    break;
            }
        }

        public void MoveCursorByTabulation(Direction direction, int tabs)
        {
            const int DEFAULT_TAB_SIZE = 8;
            int column = GetColumnIndex();
            int nextTabStop = column;

            switch (direction)
            {
                case Direction.Backward:
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

        public void MoveCursorTo(Point positionNumbers)
        {
            if (positionNumbers.X < 1) positionNumbers.X = 1;
            else if (positionNumbers.X > widthWithControl) positionNumbers.X = widthWithControl;
            if (positionNumbers.Y < 1) positionNumbers.Y = 1;

            SetCursor(screenZeroIndex + ((positionNumbers.Y - 1) * widthWithControl) + positionNumbers.X - 1);
        }

        public void MoveCursorToBeginningOfLineAbove(int lineNumberRelativeToCurrentLine)
        {
            SetCursor(GetFullIndexOfRow(GetRowIndex()) - (lineNumberRelativeToCurrentLine * widthWithControl));
        }

        public void MoveCursorToBeginningOfLineBelow(int lineNumberRelativeToCurrentLine)
        {
            SetCursor(GetFullIndexOfRow(GetRowIndex()) + (lineNumberRelativeToCurrentLine * widthWithControl));
        }

        public void MoveCursorToColumn(int columnNumber)
        {
            if (columnNumber < 1) columnNumber = 1;
            SetCursor(GetFullIndexOfRow(GetRowIndex()) + columnNumber - 1);
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
            if (linesToScroll > height)
            {
                // TODO: [P3] support for big scroll requests
                linesToScroll = height;
            }

            int newScreenZeroIndex = screenZeroIndex - (linesToScroll * widthWithControl);

            if (newScreenZeroIndex >= 0)
            {
                screenZeroIndex = newScreenZeroIndex;
                screenEndIndex = screenZeroIndex + screenLength - 1;
                return;
            }

            ResizeBuffer();

            BufferCell[] tmp = buffer;
            buffer = buffersPool.Rent(bufferSize);
            int copyStartIndex = GetFullIndexOfRow(height);
            Array.Copy(tmp, 0, buffer, copyStartIndex, bufferSize - copyStartIndex);
            ClearBufferRange(buffer, 0, copyStartIndex);

            screenZeroIndex = (screenZeroIndex + copyStartIndex) - (linesToScroll * widthWithControl);
            screenEndIndex = screenZeroIndex + screenLength - 1;
        }

        public void ScrollPageUpwards(int linesToScroll)
        {
            if (linesToScroll > height)
            {
                // TODO: [P3] support for big scroll requests
                linesToScroll = height;
            }

            screenZeroIndex = screenZeroIndex + (linesToScroll * widthWithControl);
            screenEndIndex = screenZeroIndex + screenLength - 1;

            if (screenEndIndex < bufferSize)
            {
                return;
            }

            ResizeBuffer();

            if (screenEndIndex < bufferSize)
            {
                return;
            }

            BufferCell[] tmp = buffer;
            buffer = buffersPool.Rent(bufferSize);
            int copyStartIndex = GetFullIndexOfRow(height);
            Array.Copy(tmp, copyStartIndex, buffer, 0, bufferSize - copyStartIndex);
            ClearBufferRange(buffer, bufferSize - copyStartIndex, bufferSize);
            buffersPool.Return(tmp);
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

        private void SetCursor(int newCursor)
        {
            if (newCursor < screenZeroIndex)
            {
                int screenStartRow = GetRowIndex(screenZeroIndex);
                int cursorRow = GetRowIndexFromNegative(newCursor);
                ScrollPageDownwards(screenStartRow - cursorRow);

                if (cursorRow < 0)
                {
                    int cursorBase = GetFullIndexOfRow(Math.Abs(cursorRow));
                    cursor = cursorBase + newCursor;
                    return;
                }
            }
            else if (newCursor > screenEndIndex)
            {
                int screenEndRow = GetRowIndex(screenEndIndex);
                int cursorRow = GetRowIndex(newCursor);
                ScrollPageUpwards(cursorRow - screenEndRow);
            }

            cursor = newCursor;
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
            // TODO: [P2] Create own decoder for control characters
            switch (character)
            {
                case '\b': // BS: Backspace
                    MoveCursor(Direction.Backward, 1);
                    return false;

                case '\r':
                    MoveCursorToBeginningOfLine();
                    return false;

                case '\t':
                    MoveCursorByTabulation(Direction.Forward, 1);
                    return false;

                case '\n':
                    buffer[GetControlIndex()] = new BufferCell('\n', 0);
                    MoveCursorToBeginningOfLineBelow(1);
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
        private int GetRowIndexFromNegative(int specificCursor)
        {
            return specificCursor < 0
                ? -(int)Math.Ceiling((double)Math.Abs(specificCursor) / widthWithControl)
                : GetRowIndex(specificCursor);
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

        private void SetScreenSize(int width, int height)
        {
            this.width = width;
            this.height = height;

            widthWithControl = width + ConsoleBuffer.CONTROL_LENGTH;
            screenLength = height * widthWithControl;
            screenEndIndex = screenZeroIndex + screenLength - 1;

            maxCapacity = (MAX_BUFFER_SIZE + widthWithControl) / widthWithControl * widthWithControl;
            capacityIncrement = screenLength * INITIAL_BUFFERS_COUNT;
        }

        private bool ResizeBuffer()
        {
            if (bufferSize >= MAX_BUFFER_SIZE)
            {
                return false;
            }

            BufferCell[] temp = buffer;
            int tempSize = bufferSize;
            bufferSize = Math.Min(bufferSize + capacityIncrement, maxCapacity);
            buffer = buffersPool.Rent(bufferSize);
            Array.Copy(temp, 0, buffer, 0, tempSize);
            int realBufferSize = buffer.Length / widthWithControl * widthWithControl;

            if (realBufferSize > bufferSize)
            {
                ClearBufferRange(buffer, bufferSize, realBufferSize);
                bufferSize = realBufferSize;
            }

            buffersPool.Return(temp);
            changes.Resize(bufferSize);
            return true;
        }

        private static void ClearBufferRange(BufferCell[] buffer, int startIndex, int excludedEndIndex)
        {
            for (int i = startIndex; i < excludedEndIndex; ++i)
            {
                buffer[i] = new BufferCell(BufferCell.EMPTY_CHARACTER, 0);
            }
        }
    }
}
