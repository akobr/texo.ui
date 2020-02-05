namespace BeaverSoft.Texo.Core.Console.Decoding.Ansi
{
    // TODO: [P2] Support for all SGR parameters should be added
    // https://en.wikipedia.org/wiki/ANSI_escape_code#SGR_parameters
    public enum GraphicRendition
    {
        // all attributes off
        Reset = 0,
        // Intensity: Bold
        Bold = 1,
        // Intensity: Faint     not widely supported
        Faint = 2,
        // Italic: on     not widely supported. Sometimes treated as inverse.
        Italic = 3,
        // Underline: Single     not widely supported
        Underline = 4,
        // Blink: Slow     less than 150 per minute
        BlinkSlow = 5,
        // Blink: Rapid     MS-DOS ANSI.SYS; 150 per minute or more
        BlinkRapid = 6,
        // Reverse video; inverse or reverse; swap foreground and background
        Inverse = 7,
        // Conceal     not widely supported
        Conceal = 8,
        // Crossed-out
        CrossedOut = 9,
        // Font selection (not sure which)
        DefaultFont = 10,
        // Underline: Double
        UnderlineDouble = 21,
        // Intensity: Normal     not bold and not faint
        NormalIntensity = 22,
        // Not italic, not Fraktur
        NoItalic = 23,
        // Underline: None
        NoUnderline = 24,
        // Blink: off
        NoBlink = 25,
        // Positive; the opposite of inverse?; easy to read
        Positive = 27,
        // Reveal, conceal off
        Reveal = 28,
        // Not crossed out
        NoCrossedOut = 29,

        // Set foreground color, normal intensity
        ForegroundNormalBlack = 30,
        ForegroundNormalRed = 31,
        ForegroundNormalGreen = 32,
        ForegroundNormalYellow = 33,
        ForegroundNormalBlue = 34,
        ForegroundNormalMagenta = 35,
        ForegroundNormalCyan = 36,
        ForegroundNormalWhite = 37,
        ForegroundNormalReset = 39,

        // Set background color, normal intensity
        BackgroundNormalBlack = 40,
        BackgroundNormalRed = 41,
        BackgroundNormalGreen = 42,
        BackgroundNormalYellow = 43,
        BackgroundNormalBlue = 44,
        BackgroundNormalMagenta = 45,
        BackgroundNormalCyan = 46,
        BackgroundNormalWhite = 47,
        BackgroundNormalReset = 49,

        // Overlined
        Overlined = 53,
        // Not overlined
        NoOverlined = 55,

        // Set foreground color, high intensity (aixtem)
        ForegroundBrightBlack = 90,
        ForegroundBrightRed = 91,
        ForegroundBrightGreen = 92,
        ForegroundBrightYellow = 93,
        ForegroundBrightBlue = 94,
        ForegroundBrightMagenta = 95,
        ForegroundBrightCyan = 96,
        ForegroundBrightWhite = 97,
        ForegroundBrightReset = 99,

        // Set background color, high intensity (aixterm)
        BackgroundBrightBlack = 100,
        BackgroundBrightRed = 101,
        BackgroundBrightGreen = 102,
        BackgroundBrightYellow = 103,
        BackgroundBrightBlue = 104,
        BackgroundBrightMagenta = 105,
        BackgroundBrightCyan = 106,
        BackgroundBrightWhite = 107,
        BackgroundBrightReset = 109,
    }
}
