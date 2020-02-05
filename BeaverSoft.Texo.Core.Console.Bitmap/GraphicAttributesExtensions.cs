using System.Drawing;
using BeaverSoft.Texo.Core.Console.Rendering;

namespace BeaverSoft.Texo.Core.Console.Bitmap
{
    public static class GraphicAttributesExtensions
    {
        public static Font GetFont(this GraphicAttributes attributes, Font prototype)
        {
            FontStyle fontStyles = FontStyle.Regular;

            if (attributes.IsBold) fontStyles |= FontStyle.Bold;
            if (attributes.IsItalic) fontStyles |= FontStyle.Italic;
            if (attributes.IsUnderlined) fontStyles |= FontStyle.Underline;
            if (attributes.IsCrossedOut) fontStyles |= FontStyle.Strikeout;

            return new Font(prototype, fontStyles);
        }
    }
}
