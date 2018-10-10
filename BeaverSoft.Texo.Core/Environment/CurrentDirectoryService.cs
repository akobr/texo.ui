using System;
using System.IO;

namespace BeaverSoft.Texo.Core.Environment
{
    public class CurrentDirectoryService : ICurrentDirectoryService
    {
        private readonly IEnvironmentService environment;

        public CurrentDirectoryService(IEnvironmentService environment)
        {
            this.environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public void SetCurrentDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                return;
            }

            environment.SetVariable(VariableNames.CURRENT_DIRECTORY, Path.GetFullPath(directoryPath));
        }

        public string GetCurrentDirectory()
        {

            return environment.GetVariable(VariableNames.CURRENT_DIRECTORY);
        }
    }
}
