using System.Drawing;

namespace BeaverSoft.Texo.Core.Console.Rendering.Extensions
{
    public static class RectangleExtensions
    {
        public static int GetZeroIndex(this Rectangle screen)
        {
            return screen.Y * screen.Width + screen.X;
        }
    }
}
