using System;
using System.Drawing;
using System.Text;

namespace BeaverSoft.Texo.Core.Console.Rendering
{
    public struct GraphicAttributes : IEquatable<GraphicAttributes>
    {
        public GraphicAttributes(Color foreground = default, Color background = default, GraphicStyle style = GraphicStyle.None)
        {
            Foreground = (foreground.R, foreground.G, foreground.B);
            Background = (background.R, background.G, background.B);
            Style = style;
        }

        public GraphicAttributes(
            (byte R, byte G, byte B) foreground = default,
            (byte R, byte G, byte B) background = default,
            GraphicStyle style = GraphicStyle.None)
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

        public (byte R, byte G, byte B) Foreground { get; }

        public (byte R, byte G, byte B) Background { get; }

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
            && Foreground.R == 0 && Foreground.G == 0 && Foreground.B == 0
            && Background.R == 0 && Background.G == 0 && Background.B == 0;

        public static GraphicAttributes Empty;

        public Color GetForeground()
        {
            return Color.FromArgb(Foreground.R, Foreground.G, Foreground.B);
        }

        public Color GetBackground()
        {
            return Color.FromArgb(Background.R, Background.G, Background.B);
        }

        public Font GetFont(Font prototype)
        {
            FontStyle fontStyles = FontStyle.Regular;

            if (IsBold) fontStyles |= FontStyle.Bold;
            if (IsItalic) fontStyles |= FontStyle.Italic;
            if (IsUnderlined) fontStyles |= FontStyle.Underline;
            if (IsCrossedOut) fontStyles |= FontStyle.Strikeout;

            return new Font(prototype, fontStyles);
        }

        public GraphicAttributes SetForeground(Color foreground)
        {
            return new GraphicAttributes((foreground.R, foreground.G, foreground.B), Background, Style);
        }

        public GraphicAttributes SetForeground((byte R, byte G, byte B) foreground)
        {
            return new GraphicAttributes(foreground, Background, Style);
        }

        public GraphicAttributes SetBackground(Color background)
        {
            return new GraphicAttributes(Foreground, (background.R, background.G, background.B), Style);
        }

        public GraphicAttributes SetBackground((byte R, byte G, byte B) background)
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
            builder.Append($"Foreground: #{Foreground.R:x2}{Foreground.G:x2}{Foreground.B:x2}");
            builder.Append($", Background: #{Background.R:x2}{Background.G:x2}{Background.B:x2}");

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
