using System;
using BeaverSoft.Texo.Core.Commands;
using Commands.SpinSport.Code;

namespace Commands.SpinSport
{
    public class SpinSportCommand : ModularCommand
    {
        private readonly ISolutionDirectoryProvider solutionProvider;

        public SpinSportCommand(ISolutionDirectoryProvider solutionProvider)
        {
            this.solutionProvider = solutionProvider ?? throw new ArgumentNullException(nameof(solutionProvider));

            RegisterQuery(SpinSportConstants.QUERY_CONFIG_LOCALISATION, new LocalisationCommand(solutionProvider));
            RegisterQuery(SpinSportConstants.QUERY_CONFIG_COLOUR, new ColourCommand(solutionProvider));
            RegisterQuery(SpinSportConstants.QUERY_CONFIG_COLOUR_USAGE, new ColourUsageCommand(solutionProvider));
            RegisterQuery(SpinSportConstants.QUERY_CODE, new CodeCommand());
        }
    }
}
