using System.Linq;

namespace BeaverSoft.Texo.Core.Configuration
{
    public static class OptionExtensions
    {
        public static char? GetListCharacter(this Option option)
        {
            string listInput = option.Representations.FirstOrDefault(r => r.Length == 1);

            if (string.IsNullOrWhiteSpace(listInput))
            {
                return null;
            }

            return listInput[0];
        }
    }
}
