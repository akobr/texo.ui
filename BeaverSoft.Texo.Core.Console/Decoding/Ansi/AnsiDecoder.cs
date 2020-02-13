using System;
using System.Drawing;

namespace BeaverSoft.Texo.Core.Console.Decoding.Ansi
{
    public class AnsiDecoder : EscapeCharacterDecoder, IAnsiDecoder
    {
        private IAnsiDecoderClient client;

        private int DecodeInt(string value, int defaultValue)
        {
            if (!string.IsNullOrEmpty(value)
                && int.TryParse(value, out int ret))
            {
                return ret;
            }

            return defaultValue;
        }

        private TEnum DecodeEnum<TEnum>(string value, TEnum defaultValue)
            where TEnum : struct
        {
            return Enum.TryParse(value, out TEnum decodedValue)
                ? decodedValue
                : defaultValue;
        }

        protected override void ProcessCommand(byte commandGroup, byte command, string parameter)
        {
            switch ((char)command)
            {
                case 'A':
                    client?.MoveCursor(Direction.Up, DecodeInt(parameter, 1));
                    break;

                case 'B':
                    client?.MoveCursor(Direction.Down, DecodeInt(parameter, 1));
                    break;

                case 'C':
                    client?.MoveCursor(Direction.Forward, DecodeInt(parameter, 1));
                    break;

                case 'D':
                    client?.MoveCursor(Direction.Backward, DecodeInt(parameter, 1));
                    break;

                case 'E':
                    client?.MoveCursorToBeginningOfLineBelow(DecodeInt(parameter, 1));
                    break;

                case 'F':
                    client?.MoveCursorToBeginningOfLineAbove(DecodeInt(parameter, 1));
                    break;

                case 'G':
                    client?.MoveCursorToColumn(DecodeInt(parameter, 1) - 1);
                    break;

                case 'H': // Esc[H Move cursor to upper left corner
                case 'f': // Esc[Line;Columnf Move cursor to screen location v,h
                    {
                        int separator = parameter.IndexOf(';');
                        if (separator == -1)
                        {
                            client?.MoveCursorTo(new Point(1, 1));
                        }
                        else
                        {
                            string row = parameter.Substring(0, separator);
                            string column = parameter.Substring(separator + 1, parameter.Length - separator - 1);
                            client?.MoveCursorTo(new Point(DecodeInt(column, 1), DecodeInt(row, 1)));
                        }
                    }
                    break;

                case 'I': // CHT: Cursor Forward Tabulation
                    client?.MoveCursorByTabulation(Direction.Forward, DecodeInt(parameter, 1));
                    break;

                case 'J': // Clear screen commands
                    {
                        switch (parameter)
                        {
                            case "1": // ED1: Clear screen from cursor up
                                client?.ClearScreen(DecodeEnum(parameter, ClearDirection.Backward));
                                break;

                            case "2": // ED2: Clear entire screen
                                client?.ClearScreen(DecodeEnum(parameter, ClearDirection.Both));
                                break;

                            case "":
                            case "0": // ED0: Clear screen from cursor down
                            default:
                                client?.ClearScreen(DecodeEnum(parameter, ClearDirection.Forward));
                                break;
                        }
                    }
                    break;

                case 'K': // Clear line commands
                    {
                        switch (parameter)
                        {
                            case "1": // EL1: Clear line from cursor left
                                client?.ClearLine(DecodeEnum(parameter, ClearDirection.Backward));
                                break;

                            case "2": // EL2: Clear entire line
                                client?.ClearLine(DecodeEnum(parameter, ClearDirection.Both));
                                break;

                            case "":
                            case "0": // EL0: Clear line from cursor right
                            default:
                                client?.ClearLine(DecodeEnum(parameter, ClearDirection.Forward));
                                break;
                        }
                    }
                    break;

                case 'S':
                    client?.ScrollPageUpwards(DecodeInt(parameter, 1));
                    break;

                case 'T':
                    client?.ScrollPageDownwards(DecodeInt(parameter, 1));
                    break;

                case 'X': // ECH: Erase n character(s)
                    client?.EraseCharacters(DecodeInt(parameter, 1));
                    break;

                case 'Z': // CBT: Cursor Backward Tabulation
                    client?.MoveCursorByTabulation(Direction.Backward, DecodeInt(parameter, 1));
                    break;

                case 'm':
                    {
                        // TODO: [P2] this needs to be reviewed and needs support for 8-bit and 24-bit
                        string[] commands = parameter.Split(';');
                        GraphicRendition[] renditionCommands = new GraphicRendition[commands.Length];
                        for (int i = 0; i < commands.Length; ++i)
                        {
                            renditionCommands[i] = (GraphicRendition)DecodeInt(commands[i], 0);
                        }
                        client?.SetGraphicRendition(renditionCommands);
                    }
                    break;

                case 'n':
                    if (parameter == "6")
                    {
                        Point cursorPosition = client?.GetCursorPosition() ?? Point.Empty;
                        cursorPosition.X++;
                        cursorPosition.Y++;
                        string row = cursorPosition.Y.ToString();
                        string column = cursorPosition.X.ToString();
                        byte[] output = new byte[2 + row.Length + 1 + column.Length + 1];
                        int i = 0;
                        output[i++] = EscapeCharacter;
                        output[i++] = LeftBracketCharacter;
                        foreach (char c in row)
                        {
                            output[i++] = (byte)c;
                        }
                        output[i++] = (byte)';';
                        foreach (char c in column)
                        {
                            output[i++] = (byte)c;
                        }
                        output[i++] = (byte)'R';
                        OnOutput(output);
                    }
                    break;

                case 's':
                    client?.SaveCursor();
                    break;

                case 'u':
                    client?.RestoreCursor();
                    break;

                case 'l':
                    switch (parameter)
                    {
                        case "20":
                            // LMN: Set line feed mode
                            client?.ChangeMode(AnsiMode.LineFeed);
                            break;

                        case "?1":
                            // DECCKM: Set cursor key to cursor
                            client?.ChangeMode(AnsiMode.CursorKeyToCursor);
                            break;

                        case "?2":
                            // DECANM: Set VT52 (versus ANSI)
                            client?.ChangeMode(AnsiMode.VT52);
                            break;

                        case "?3":
                            // DECCOLM: Set number of columns to 80
                            client?.ChangeMode(AnsiMode.Columns80);
                            break;

                        case "?4":
                            // DECSCLM: Set jump scrolling
                            client?.ChangeMode(AnsiMode.JumpScrolling);
                            break;

                        case "?5":
                            // DECSCNM: Set normal video on screen
                            client?.ChangeMode(AnsiMode.NormalVideo);
                            break;

                        case "?6":
                            // DECOM: Set origin to absolute
                            client?.ChangeMode(AnsiMode.OriginIsAbsolute);
                            break;

                        case "?7":
                            // DECAWM: Reset auto-wrap mode (disable line wrap)
                            client?.ChangeMode(AnsiMode.DisableLineWrap);
                            break;

                        case "?8":
                            // DECARM: Reset auto-repeat mode
                            client?.ChangeMode(AnsiMode.DisableAutoRepeat);
                            break;

                        case "?9":
                            // DECINLM: Reset interlacing mode
                            client?.ChangeMode(AnsiMode.DisableInterlacing);
                            break;

                        case "?25":
                            client?.ChangeMode(AnsiMode.HideCursor);
                            break;

                        default:
                            throw new InvalidCommandException(command, parameter);
                    }
                    break;

                case 'h':
                    switch (parameter)
                    {
                        case "":
                            // DECANM: Set ANSI (versus VT52)
                            client?.ChangeMode(AnsiMode.ANSI);
                            break;

                        case "20":
                            // LMN: Set new line mode
                            client?.ChangeMode(AnsiMode.NewLine);
                            break;

                        case "?1":
                            // DECCKM: Set cursor key to application
                            client?.ChangeMode(AnsiMode.CursorKeyToApplication);
                            break;

                        case "?3":
                            // DECCOLM: Set number of columns to 132
                            client?.ChangeMode(AnsiMode.Columns132);
                            break;

                        case "?4":
                            // DECSCLM: Set smooth scrolling
                            client?.ChangeMode(AnsiMode.SmoothScrolling);
                            break;

                        case "?5":
                            // DECSCNM: Set reverse video on screen
                            client?.ChangeMode(AnsiMode.ReverseVideo);
                            break;

                        case "?6":
                            // DECOM: Set origin to relative
                            client?.ChangeMode(AnsiMode.OriginIsRelative);
                            break;

                        case "?7":
                            // DECAWM: Set auto-wrap mode (enable line wrap)
                            client?.ChangeMode(AnsiMode.LineWrap);
                            break;

                        case "?8":
                            // DECARM: Set auto-repeat mode
                            client?.ChangeMode(AnsiMode.AutoRepeat);
                            break;

                        case "?9":
                            // DECINLM: Set interlacing mode
                            client?.ChangeMode(AnsiMode.Interlacing);
                            break;

                        case "?25":
                            client?.ChangeMode(AnsiMode.ShowCursor);
                            break;

                        default:
                            throw new InvalidCommandException(command, parameter);
                    }
                    break;

                case '>':
                    // Set numeric keypad mode
                    client?.ChangeMode(AnsiMode.NumericKeypad);
                    break;

                case '=':
                    client?.ChangeMode(AnsiMode.AlternateKeypad);
                    // Set alternate keypad mode (rto: non-numeric, presumably)
                    break;

                case ']':
                case '\a':
                    // 	Operating System Command
                    // '\a'
                    break;

                case '\\': // String Terminator
                    // Terminates strings in other controls
                    break;

                default:
                    throw new InvalidCommandException(command, parameter);
            }
        }

        protected override bool IsValidOneCharacterCommand(byte command)
        {
            return command == '='
                || command == '>'
                || command == 'A'
                || command == 'B'
                || command == 'C'
                || command == 'D'
                || command == 'H'
                || command == 'J'
                || command == 'K';
        }

        protected override void OnCharacters(char[] characters)
        {
            client?.Characters(characters);
        }

        private static string[] FUNCTIONKEY_MAP = {
        //   F1    F2    F3    F4    F5    F6    F7    F8    F9    F10   F11   F12
            "1P", "1Q", "1R", "1S", "15", "17", "18", "19", "20", "21", "23", "24",
        //   F13   F14   F15   F16   F17   F18   F19   F20   F21   F22   F23   F24
            "25", "26", "28", "29", "31", "32", "33", "34", "23", "24", "25", "26" };

        // TODO: [P3] review and refactor
        public override bool KeyPressed(Keys modifiers, Keys key)
        {
            if ((int)Keys.F1 <= (int)key && (int)key <= (int)Keys.F24)
            {
                byte[] r = new byte[5];
                r[0] = 0x1B;
                r[1] = (byte)'[';
                int n = (int)key - (int)Keys.F1;
                if ((modifiers & Keys.Shift) != Keys.None)
                    n += 10;
                char tail;
                if (n >= 20)
                    tail = (modifiers & Keys.Control) != Keys.None ? '@' : '$';
                else
                    tail = (modifiers & Keys.Control) != Keys.None ? '^' : '~';
                string f = FUNCTIONKEY_MAP[n];
                r[2] = (byte)f[0];
                r[3] = (byte)f[1];
                r[4] = (byte)tail;
                OnOutput(r);
                return true;
            }
            else if (key == Keys.Left || key == Keys.Right || key == Keys.Up || key == Keys.Down)
            {
                byte[] r = new byte[3];
                r[0] = 0x1B;
                r[1] = (byte)'[';
                switch (key)
                {
                    case Keys.Up:
                        r[2] = (byte)'A';
                        break;
                    case Keys.Down:
                        r[2] = (byte)'B';
                        break;
                    case Keys.Right:
                        r[2] = (byte)'C';
                        break;
                    case Keys.Left:
                        r[2] = (byte)'D';
                        break;
                    default:
                        return false;
                }
                OnOutput(r);
                return true;
            }
            else if (modifiers.HasFlag(Keys.Control))
            {
                byte controlCode;
                // https://www.windmill.co.uk/ascii-control-codes.html
                switch (key)
                {
                    case Keys.D2: // ^@	NUL	000	00	Null character
                        controlCode = 0;
                        break;
                    case Keys.A: // ^A	SOH	001	01	Start of Header
                        controlCode = 1;
                        break;
                    case Keys.B: // ^B	STX	002	02	Start of Text
                        controlCode = 2;
                        break;
                    case Keys.C: // ^C	ETX	003	03	End of Text
                        controlCode = 3;
                        break;
                    case Keys.D: // ^D	EOT	004	04	End of Transmission
                        controlCode = 4;
                        break;
                    case Keys.E: // ^E	ENQ	005	05	Enquiry
                        controlCode = 5;
                        break;
                    case Keys.F: // ^F	ACK	006	06	Acknowledge
                        controlCode = 6;
                        break;
                    case Keys.G: // ^G	BEL	007	07	Bell
                        controlCode = 7;
                        break;
                    case Keys.H: // ^H	BS	008	08	Backspace
                        controlCode = 8;
                        break;
                    case Keys.I: // ^I	HT	009	09	Horizontal tab
                        controlCode = 9;
                        break;
                    case Keys.J: // ^J	LF	010	0A	Line feed
                        controlCode = 10;
                        break;
                    case Keys.K: // ^K	VT	011	0B	Vertical tab
                        controlCode = 11;
                        break;
                    case Keys.L: // ^L	FF	012	0C	Form feed
                        controlCode = 12;
                        break;
                    case Keys.M: // ^M	CR	013	0D	Carriage return
                        controlCode = 13;
                        break;
                    case Keys.N: // ^N	SO	014	0E	Shift out
                        controlCode = 14;
                        break;
                    case Keys.O: // ^O	SI	015	0F	Shift in
                        controlCode = 15;
                        break;
                    case Keys.P: // ^P	DLE	016	10	Data link escape
                        controlCode = 16;
                        break;
                    case Keys.Q: // ^Q	DCL	017	11	Xon (transmit on)
                        controlCode = 17;
                        break;
                    case Keys.R: // ^R	DC2	018	12	Device control 2
                        controlCode = 18;
                        break;
                    case Keys.S: // ^S	DC3	019	13	Xoff (transmit off)
                        controlCode = 19;
                        break;
                    case Keys.T: // ^T	DC4	020	14	Device control 4
                        controlCode = 20;
                        break;
                    case Keys.U: // ^U	NAK	021	15	Negative acknowledge
                        controlCode = 21;
                        break;
                    case Keys.V: // ^V	SYN	022	16	Synchronous idle
                        controlCode = 22;
                        break;
                    case Keys.W: // ^W	ETB	023	17	End of transmission
                        controlCode = 23;
                        break;
                    case Keys.X: // ^X	CAN	024	18	Cancel
                        controlCode = 24;
                        break;
                    case Keys.Y: // ^Y	EM	025	19	End of medium
                        controlCode = 25;
                        break;
                    case Keys.Z: // ^Z	SUB	026	1A	Substitute
                        controlCode = 26;
                        break;
                    case Keys.OemOpenBrackets: // ^[	ESC	027	1B	Escape
                        controlCode = 27;
                        break;
                    case Keys.OemBackslash: // ^\	FS	028	1C	File separator
                    case Keys.OemPipe:
                    // case Keys.Oem5:
                        controlCode = 28;
                        break;
                    case Keys.OemCloseBrackets: // ^]	GS	029	1D	Group separator
                    // case Keys.Oem6:
                        controlCode = 29;
                        break;
                    case Keys.D6: // ^^	RS	030	1E	Record separator
                        controlCode = 30;
                        break;
                    case Keys.Oemplus: // ^_	US	031	1F	Unit separator
                    case Keys.OemMinus:
                        controlCode = 31;
                        break;
                    default:
                        return false;
                }
                OnOutput(new byte[] { controlCode });
                return true;
            }
            else
            {
                byte[] r = new byte[4];
                r[0] = 0x1B;
                r[1] = (byte)'[';
                r[3] = (byte)'~';
                switch (key)
                {
                    case Keys.Home:
                        r[2] = (byte)'1';
                        break;
                    case Keys.Insert:
                        r[2] = (byte)'2';
                        break;
                    case Keys.Delete:
                        r[2] = (byte)'3';
                        break;
                    case Keys.End:
                        r[2] = (byte)'4';
                        break;
                    case Keys.PageUp:
                        r[2] = (byte)'5';
                        break;
                    case Keys.PageDown:
                        r[2] = (byte)'6';
                        break;
                    case Keys.Enter:
                        r = new byte[] { 13 };
                        break;
                    case Keys.Escape:
                        r = new byte[] { 0x1B };
                        break;
                    case Keys.Tab:
                        r = new byte[] { (byte)'\t' };
                        break;
                    default:
                        return false;
                }
                OnOutput(r);
                return true;
            }
        }

        public void Subscribe(IAnsiDecoderClient newClient)
        {
            client = newClient;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                client = null;
            }
        }
    }
}
