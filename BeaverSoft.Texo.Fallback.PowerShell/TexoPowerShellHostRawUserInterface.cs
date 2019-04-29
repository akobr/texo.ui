using System;
using System.Management.Automation.Host;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Fallback.PowerShell
{
    public class TexoPowerShellHostRawUserInterface : PSHostRawUserInterface
    {
        private const int DEFAULT_LINE_LENGHT = 126;
        private const int DEFAULT_WINDOW_LINE_COUNT = 30;
        private const int DEFAULT_BUFFER_HEIGHT = 300;

        private readonly ILogService logger;
        private readonly Size windowSize = new Size(DEFAULT_LINE_LENGHT, DEFAULT_WINDOW_LINE_COUNT);

        private Size bufferSize;
        private BufferCell[,] buffer;

        public TexoPowerShellHostRawUserInterface(ILogService logger)
        {
            bufferSize = new Size(DEFAULT_LINE_LENGHT, DEFAULT_BUFFER_HEIGHT);
            buffer = new BufferCell[bufferSize.Width, bufferSize.Height];
            this.logger = logger;
        }

        public override ConsoleColor BackgroundColor
        {
            get
            {
                return ConsoleColor.Black;
            }
            set
            {
                logger.Debug("PSHostRawUserInterface: BackgroundColor", value);
            }
        }

        public override ConsoleColor ForegroundColor
        {
            get
            {
                return ConsoleColor.White;
            }
            set
            {
                logger.Debug("PSHostRawUserInterface: ForegroundColor", value);
            }
        }

        public override Size BufferSize
        {
            get
            {
                return bufferSize;
            }
            set
            {
                logger.Debug("PSHostRawUserInterface: BufferSize", value);
            }
        }

        public override Coordinates CursorPosition
        {
            get
            {
                return new Coordinates(0, 0);
            }
            set
            {
                logger.Debug("PSHostRawUserInterface: CursorPosition", value);
            }
        }

        public override int CursorSize
        {
            get
            {
                return 0;
            }
            set
            {
                logger.Debug("PSHostRawUserInterface: CursorSize", value);
            }
        }


        public override bool KeyAvailable
        {
            get
            {
                return false;
            }
        }

        public override Size MaxPhysicalWindowSize
        {
            get
            {
                return windowSize;
            }
        }

        public override Size MaxWindowSize
        {
            get
            {
                return windowSize;
            }
        }

        public override Coordinates WindowPosition
        {
            get
            {
                return new Coordinates(0, 0);
            }
            set
            {
                logger.Debug("PSHostRawUserInterface: WindowPosition", value);
            }
        }


        public override Size WindowSize
        {
            get
            {
                return windowSize;
            }
            set
            {
                logger.Debug("PSHostRawUserInterface: WindowSize", value);
            }
        }

        public override string WindowTitle
        {
            get
            {
                return "TexoUi Host";
            }
            set
            {
                logger.Debug("PSHostRawUserInterface: WindowTitle", value);
            }
        }

        public override void FlushInputBuffer()
        {
            // No operation
        }

        public override BufferCell[,] GetBufferContents(Rectangle rectangle)
        {
            return new BufferCell[rectangle.Right - rectangle.Left, rectangle.Bottom - rectangle.Top];
        }

        public override KeyInfo ReadKey(ReadKeyOptions options)
        {
            throw new NotImplementedException();
        }

        public override void ScrollBufferContents(Rectangle source, Coordinates destination, Rectangle clip, BufferCell fill)
        {
            // no operation
        }

        public override void SetBufferContents(Coordinates origin, BufferCell[,] contents)
        {
            int xMax = contents.GetUpperBound(0);
            int yMax = contents.GetUpperBound(1);

            for (int x = 0; x <= xMax; x++)
            {
                for (int y = 0; y <= yMax; y++)
                {
                    buffer[origin.X + x, origin.Y + y] = contents[x, y];
                }
            }
        }

        public override void SetBufferContents(Rectangle rectangle, BufferCell fill)
        {
            for (int x = rectangle.Left; x <= rectangle.Right; x++)
            {
                for (int y = rectangle.Top; y < rectangle.Bottom; y++)
                {
                    buffer[x, y] = fill;
                }
            }
        }
    }
}
