using System;
using System.Drawing;
using System.Text;

namespace BeaverSoft.Texo.Core.Console.Rendering
{
    public struct GraphicAttributes : IEquatable<GraphicAttributes>
    {
        public GraphicAttributes(Color foreground = default, Color background = default, GraphicStyle style = GraphicStyle.None)
        {
            Foreground = foreground;
            Background = background;
            Style = style;
        }

        public GraphicAttributes(GraphicAttributes prototype)
        {
            Foreground = prototype.Foreground;
            Background = prototype.Background;
            Style = prototype.Style;
        }

        public Color Foreground { get; }

        public Color Background { get; }

        public GraphicStyle Style { get; }

        public bool IsRegular
            => Style == GraphicStyle.None
            || (Style.HasFlag(GraphicStyle.Bold) && Style.HasFlag(GraphicStyle.Faint));

        public bool IsBold => Style.HasFlag(GraphicStyle.Bold) && !Style.HasFlag(GraphicStyle.Faint);

        public bool IsFaint => Style.HasFlag(GraphicStyle.Faint) && !Style.HasFlag(GraphicStyle.Bold);

        public bool IsItalic => Style.HasFlag(GraphicStyle.Italic);

        public bool IsUnderlined => Style.HasFlag(GraphicStyle.Underline);

        public bool IsCrossedOut => Style.HasFlag(GraphicStyle.CrossOut);

        public bool IsOverlined => Style.HasFlag(GraphicStyle.Overline);

        public bool IsBlinking => Style.HasFlag(GraphicStyle.Blink);

        public bool IsConcealed => Style.HasFlag(GraphicStyle.Conceal);

        public bool IsEmpty =>
            Style == GraphicStyle.None
            && Foreground.IsEmpty
            && Background.IsEmpty;

        public static GraphicAttributes Empty;

        public GraphicAttributes SetForeground(Color foreground)
        {
            return new GraphicAttributes(foreground, Background, Style);
        }

        public GraphicAttributes SetBackground(Color background)
        {
            return new GraphicAttributes(Foreground, background, Style);
        }

        public GraphicAttributes SetStyle(GraphicStyle style)
        {
            return new GraphicAttributes(Foreground, Background, style);
        }

        public GraphicAttributes AddStyle(GraphicStyle style)
        {
            return new GraphicAttributes(Foreground, Background, Style | style);
        }

        public GraphicAttributes RemoveStyle(GraphicStyle style)
        {
            return new GraphicAttributes(Foreground, Background, Style & ~style);
        }

        public GraphicAttributes Reverse()
        {
            return new GraphicAttributes(Background, Foreground, Style);
        }

        public static bool operator ==(GraphicAttributes left, GraphicAttributes right)
        {
            return left.Style == right.Style
                && left.Foreground == right.Foreground
                && left.Background == right.Background;
        }

        public static bool operator !=(GraphicAttributes left, GraphicAttributes right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            return obj is GraphicAttributes other && Equals(other);
        }

        public bool Equals(GraphicAttributes other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return new { Style, Foreground, Background }.GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"Foreground: {Foreground}");
            builder.Append($", Background: {Background}");

            if (IsRegular) builder.Append(", Regular");
            else if (IsBold) builder.Append(", Bold");
            else if (IsFaint) builder.Append(", Faint");
            if (IsItalic) builder.Append(", Italic");
            if (IsUnderlined) builder.Append(", Underline");
            if (IsCrossedOut) builder.Append(", CrossOut");
            if (IsOverlined) builder.Append(", Overline");
            if (IsBlinking) builder.Append(", Blink");
            if (IsConcealed) builder.Append(", Conceal");

            return builder.ToString();
        }
    }
}
