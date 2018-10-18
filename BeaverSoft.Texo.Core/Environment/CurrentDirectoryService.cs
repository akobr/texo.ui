using System;
using System.IO;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.Core.Environment
{
    public class CurrentDirectoryService : ICurrentDirectoryService
    {
        private readonly IEnvironmentService environment;
        private readonly IViewService view;

        public CurrentDirectoryService(IEnvironmentService environment, IViewService view)
        {
            this.environment = environment ?? throw new ArgumentNullException(nameof(environment));
            this.view = view ?? throw new ArgumentNullException(nameof(view));
        }

        public void SetCurrentDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                return;
            }

            string fullPath = Path.GetFullPath(directoryPath);
            environment.SetVariable(VariableNames.CURRENT_DIRECTORY, fullPath);
            view.UpdateCurrentDirectory(fullPath);
        }

        public string GetCurrentDirectory()
        {
            return environment.GetVariable(VariableNames.CURRENT_DIRECTORY);
        }
    }
}
