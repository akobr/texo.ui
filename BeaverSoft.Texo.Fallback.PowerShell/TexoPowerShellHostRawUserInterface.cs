using System;
using System.Management.Automation.Host;

namespace BeaverSoft.Texo.Fallback.PowerShell
{
    public class TexoPowerShellHostRawUserInterface : PSHostRawUserInterface
    {
        public override ConsoleColor BackgroundColor { get; set; }

        public override Size BufferSize { get; set; }

        public override Coordinates CursorPosition { get; set; }

        public override int CursorSize { get; set; }

        public override ConsoleColor ForegroundColor { get; set; }

        public override bool KeyAvailable { get; }

        public override Size MaxPhysicalWindowSize { get; }

        public override Size MaxWindowSize { get; }

        public override Coordinates WindowPosition { get; set; }

        public override Size WindowSize { get; set; }

        public override string WindowTitle { get; set; }

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
