using System;
using System.Management.Automation.Host;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Fallback.PowerShell
{
    public class TexoPowerShellHostRawUserInterface : PSHostRawUserInterface
    {
        private readonly Size fakeWindowSize = new Size(1024, 1024);
        private readonly ILogService logger;

        public TexoPowerShellHostRawUserInterface(ILogService logger)
        {
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
                return fakeWindowSize;
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
                return fakeWindowSize;
            }
        }

        public override Size MaxWindowSize
        {
            get
            {
                return fakeWindowSize;
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
                return fakeWindowSize;
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
            throw new NotImplementedException();
        }

        public override BufferCell[,] GetBufferContents(Rectangle rectangle)
        {
            throw new NotImplementedException();
        }

        public override KeyInfo ReadKey(ReadKeyOptions options)
        {
            throw new NotImplementedException();
        }

        public override void ScrollBufferContents(Rectangle source, Coordinates destination, Rectangle clip, BufferCell fill)
        {
            throw new NotImplementedException();
        }

        public override void SetBufferContents(Coordinates origin, BufferCell[,] contents)
        {
            throw new NotImplementedException();
        }

        public override void SetBufferContents(Rectangle rectangle, BufferCell fill)
        {
            throw new NotImplementedException();
        }
    }
}
