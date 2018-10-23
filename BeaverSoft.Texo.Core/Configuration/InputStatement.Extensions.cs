using System.Linq;

namespace BeaverSoft.Texo.Core.Configuration
{
    public static class InputStatementExtensions
    {
        public static string GetMainRepresentation(this InputStatement statement)
        {
            return statement?.Representations?.FirstOrDefault() ?? string.Empty;
        }

        public static bool HasParameters(this InputStatement statement)
        {
            return statement?.Parameters?.Count > 0;
        }
    }
}
