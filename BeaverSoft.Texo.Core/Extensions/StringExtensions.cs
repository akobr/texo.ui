using System;

namespace BeaverSoft.Texo.Core.Extensions
{
    public static class StringExtensions
    {
        public static bool IsTrue(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "t", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "1");
        }

        public static bool IsFalse(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            return string.Equals(value, "false", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "f", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "0");
        }
    }
}
