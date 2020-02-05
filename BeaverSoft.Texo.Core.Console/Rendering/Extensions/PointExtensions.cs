using System.Drawing;

namespace BeaverSoft.Texo.Core.Console.Rendering.Extensions
{
    public static class PointExtensions
    {
        public static int GetIndex(this Point cursor, int width)
        {
            return cursor.Y * width + cursor.X;
        }
    }
}
