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
            "11", "12", "13", "14", "15", "17", "18", "19", "20", "21", "23", "24",
        //   F13   F14   F15   F16   F17   F18   F19   F20   F21   F22
            "25", "26", "28", "29", "31", "32", "33", "34", "23", "24" };

        // TODO: [P3] review and refactor
        public override bool KeyPressed(Keys modifiers, Keys key)
        {
            if ((int)Keys.F1 <= (int)key && (int)key <= (int)Keys.F12)
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
                //if ( _cursorKeyMode == TerminalMode.Normal )
                r[1] = (byte)'[';
                //else
                //    r[1] = (byte) 'O';

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
                        throw new ArgumentException("unknown cursor key code", "key");
                }
                OnOutput(r);
                return true;
            }
            else
            {
                byte[] r = new byte[4];
                r[0] = 0x1B;
                r[1] = (byte)'[';
                r[3] = (byte)'~';
                if (key == Keys.Insert)
                {
                    r[2] = (byte)'1';
                }
                else if (key == Keys.Home)
                {
                    r[2] = (byte)'2';
                }
                else if (key == Keys.PageUp)
                {
                    r[2] = (byte)'3';
                }
                else if (key == Keys.Delete)
                {
                    r[2] = (byte)'4';
                }
                else if (key == Keys.End)
                {
                    r[2] = (byte)'5';
                }
                else if (key == Keys.PageDown)
                {
                    r[2] = (byte)'6';
                }
                else if (key == Keys.Enter)
                {
                    //return new byte[] { 0x1B, (byte) 'M', (byte) '~' };
                    //r[1] = (byte) 'O';
                    //r[2] = (byte) 'M';
                    //return new byte[] { (byte) '\r', (byte) '\n' };
                    r = new byte[] { 13 };
                }
                else if (key == Keys.Escape)
                {
                    r = new byte[] { 0x1B };
                }
                else if (key == Keys.Tab)
                {
                    r = new byte[] { (byte)'\t' };
                }
                else
                {
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
