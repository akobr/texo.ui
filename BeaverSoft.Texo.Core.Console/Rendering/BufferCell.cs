using System;

namespace BeaverSoft.Texo.Core.Console.Rendering
{
    public struct BufferCell : IEquatable<BufferCell>, IEquatable<char>
    {
        public const char EMPTY_CHARACTER = '\0';

        public BufferCell(char character, byte styleId = 0)
        {
            Character = character;
            StyleId = styleId;
        }

        public char Character { get; }

        public byte StyleId { get; }

        public bool IsEmpty => Character == EMPTY_CHARACTER;

        public static BufferCell Empty;

        public BufferCell SetCharacter(char character)
        {
            return new BufferCell(character, StyleId);
        }

        public BufferCell SetStyleId(byte styleId)
        {
            return new BufferCell(Character, styleId);
        }

        public static bool operator ==(BufferCell left, BufferCell right)
        {
            return left.Character == right.Character
                && left.StyleId == right.StyleId;
        }

        public static bool operator !=(BufferCell left, BufferCell right)
        {
            return !(left == right);
        }

        public static bool operator ==(BufferCell left, char character)
        {
            return left.Character == character;
        }

        public static bool operator !=(BufferCell left, char character)
        {
            return !(left == character);
        }

        public override bool Equals(object obj)
        {
            return obj is BufferCell other && Equals(other);
        }

        public bool Equals(BufferCell other)
        {
            return this == other;
        }

        public bool Equals(char character)
        {
            return Character == character;
        }

        public override int GetHashCode()
        {
            return new { Character, StyleId }.GetHashCode();
        }

        public override string ToString()
        {
            if (IsEmpty)
            {
                return $"\\0, Style: {StyleId}";
            }

            return $"'{Character}', {(int)Character}, \\u{(int)Character:x4}, Style: {StyleId}";
        }
    }
}
