using System;
using BeaverSoft.Texo.Core.Commands;

namespace Commands.SpinSport
{
    public class SpinSportCommand : IntersectionCommand
    {
        private readonly ISolutionDirectoryProvider solutionProvider;

        public SpinSportCommand(ISolutionDirectoryProvider solutionProvider)
        {
            this.solutionProvider = solutionProvider ?? throw new ArgumentNullException(nameof(solutionProvider));

            RegisterCommand(SpinSportConstants.QUERY_CONFIG_LOCALISATION, new LocalisationCommand(solutionProvider));
            RegisterCommand(SpinSportConstants.QUERY_CONFIG_COLOUR, new ColourCommand(solutionProvider));
            RegisterCommand(SpinSportConstants.QUERY_CONFIG_COLOUR_USAGE, new ColourUsageCommand(solutionProvider));
        }
    }
}
