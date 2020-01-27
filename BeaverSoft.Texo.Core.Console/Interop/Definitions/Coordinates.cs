using System.Runtime.InteropServices;

namespace BeaverSoft.Texo.Core.Console.Interop.Definitions
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Coordinates
    {
        public short X;
        public short Y;
    }
}
