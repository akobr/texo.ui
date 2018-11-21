using System.Security;

namespace BeaverSoft.Texo.Fallback.PowerShell.Extensions
{
    public static class StringExtensions
    {
        public static SecureString ToSecureString(this string text)
        {
            if (text == null)
            {
                return null;
            }

            SecureString result = new SecureString();

            foreach (char character in text.ToCharArray())
            {
                result.AppendChar(character);
            }

            return result;
        }
    }
}
