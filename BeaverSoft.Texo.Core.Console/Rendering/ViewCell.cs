using System;

namespace BeaverSoft.Texo.Core.Console.Rendering
{
    public struct ViewCell : IEquatable<ViewCell>, IEquatable<char>
    {
        public const char EMPTY_CHARACTER = '\0';

        public ViewCell(char character, GraphicAttributes attributes = default)
        {
            Character = character;
            Attributes = attributes;
        }

        public char Character { get; }

        public GraphicAttributes Attributes { get; }

        public bool IsEmpty => Character == EMPTY_CHARACTER;

        public static ViewCell Empty;

        public ViewCell SetCharacter(char character)
        {
            return new ViewCell(character, Attributes);
        }

        public ViewCell SetAttributes(GraphicAttributes attributes)
        {
            return new ViewCell(Character, attributes);
        }

        public static bool operator ==(ViewCell left, ViewCell right)
        {
            return left.Character == right.Character
                && left.Attributes == right.Attributes;
        }

        public static bool operator !=(ViewCell left, ViewCell right)
        {
            return !(left == right);
        }

        public static bool operator ==(ViewCell left, char character)
        {
            return left.Character == character;
        }

        public static bool operator !=(ViewCell left, char character)
        {
            return !(left == character);
        }

        public override bool Equals(object obj)
        {
            return obj is ViewCell other && Equals(other);
        }

        public bool Equals(ViewCell other)
        {
            return this == other;
        }

        public bool Equals(char character)
        {
            return Character == character;
        }

        public override int GetHashCode()
        {
            return new { Character, Attributes }.GetHashCode();
        }

        public override string ToString()
        {
            return $"'{Character}', {Character:D}, \\u{Character:x}, {Attributes}";
        }
    }
}
