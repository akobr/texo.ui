using System.IO;
using StrongBeaver.Core.Extensions;

namespace BeaverSoft.Texo.Commands.NugetManager.Extenssions
{
    public static class StringExtenssions
    {
        public static int GetPathHash(this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return 0;
            }

            return Path.GetFullPath(path)
                .ToLowerInvariant()
                .GetHashCode();
        }

        public static string GetPathId(this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return string.Empty;
            }

            return Path.GetFullPath(path)
                .ToLowerInvariant()
                .CalculateHashMd5();
        }
    }
}
