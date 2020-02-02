using System;

namespace BeaverSoft.Texo.Core.Console.Rendering
{
    [Flags]
    public enum GraphicStyle : byte
    {
        None = 0,
        Faint = 1,
        Bold = 2,
        Italic = 4,
        Underline = 8,
        CrossOut = 16,
        Overline = 32,
        Blink = 64,
        Conceal = 128
    }
}
