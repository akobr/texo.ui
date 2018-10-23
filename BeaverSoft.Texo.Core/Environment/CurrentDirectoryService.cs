using System;
using System.IO;
using BeaverSoft.Texo.Core.View;

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

            string fullPath = Path.GetFullPath(directoryPath);
            environment.SetVariable(VariableNames.CURRENT_DIRECTORY, fullPath);
        }

        public string GetCurrentDirectory()
        {
            return environment.GetVariable(VariableNames.CURRENT_DIRECTORY);
        }
    }
}
