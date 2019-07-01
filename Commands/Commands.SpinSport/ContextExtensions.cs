using System.Text.RegularExpressions;
using BeaverSoft.Texo.Core.Commands;

namespace Commands.SpinSport
{
    public static class ContextExtensions
    {
        public static Regex GetFilterRegex(this CommandContext context)
        {
            string filter = ".*";

            if (context.HasParameter(SpinSportConstants.PARAMETER_FILTER))
            {
                filter = context.GetParameterValue(SpinSportConstants.PARAMETER_FILTER);
                filter = filter.Replace("*", ".*");
            }

            return new Regex($"^{filter}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }
    }
}
