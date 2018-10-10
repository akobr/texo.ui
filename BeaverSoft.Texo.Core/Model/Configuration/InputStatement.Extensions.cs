using System.Linq;

namespace BeaverSoft.Texo.Core.Model.Configuration
{
    public static class InputStatementExtensions
    {
        public static string GetMainRepresentation(this IInputStatement statement)
        {
            return statement?.Representations?.FirstOrDefault() ?? string.Empty;
        }

        public static bool HasParameters(this IInputStatement statement)
        {
            return statement?.Parameters?.Count > 0;
        }
    }
}
