using System.Text.RegularExpressions;

namespace BeaverSoft.Texo.Core.Markdown
{
    public static class Markdown
    {
        private static readonly Regex escape = new Regex("([!\"#$%&'()*+,\\-\\.\\/:;<=>?@[\\\\\\]^_`{|}~])", RegexOptions.Compiled);

        public static string Encode(string text)
        {
            return escape.Replace(text, @"\$1");
        }
    }
}
