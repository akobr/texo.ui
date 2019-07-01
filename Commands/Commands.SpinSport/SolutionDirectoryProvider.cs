using System;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Path;

namespace Commands.SpinSport
{
    public class SolutionDirectoryProvider : ISolutionDirectoryProvider
    {
        private readonly IEnvironmentService environment;

        public SolutionDirectoryProvider(IEnvironmentService environment)
        {
            this.environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public string Get()
        {
            return environment.GetVariable("SOLUTION_DIRECTORY", PathConstants.RELATIVE_CURRENT_DIRECTORY);
        }
    }
}
