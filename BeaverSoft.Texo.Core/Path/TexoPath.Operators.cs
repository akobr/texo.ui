using System;

namespace BeaverSoft.Texo.Core.Path
{
    public partial class TexoPath
    {
        public static implicit operator TexoPath(string path)
        {
            return new TexoPath(path);
        }

        public static implicit operator TexoPath(Uri path)
        {
            return new TexoPath(path);
        }

        public static explicit operator string(TexoPath path)
        {
            return path.GetAbsolutePath();
        }

        public static explicit operator Uri(TexoPath path)
        {
            return path?.ToUri();
        }

        public static TexoPath operator +(TexoPath first, TexoPath second)
        {
            return first.Combine(second);
        }
    }
}
