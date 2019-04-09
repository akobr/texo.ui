using System;
using System.IO;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Extensions;
using BeaverSoft.Texo.Core.Path;
using BeaverSoft.Texo.Core.Result;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Core.Environment
{
    public class CurrentDirectoryCommand : ICommand
    {
        private const string PARAMETER_PATH = "path";

        private readonly IEnvironmentService service;
        private readonly ILogService logger;

        public CurrentDirectoryCommand(IEnvironmentService service, ILogService logger)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
            this.logger = logger;
        }

        public ICommandResult Execute(CommandContext context)
        {
            string currentPath = service.GetVariable(VariableNames.CURRENT_DIRECTORY);

            if (!context.Parameters.TryGetValue(PARAMETER_PATH, out ParameterContext parameter))
            {
                return new TextResult(currentPath);
            }

            foreach (string path in parameter.GetValues())
            {
                try
                {
                    if (!path.IsRelativePath())
                    {
                        currentPath = ChangePathIfExists(currentPath, path);
                        continue;
                    }

                    string newPath = System.IO.Path.Combine(currentPath, path);
                    currentPath = ChangePathIfExists(currentPath, newPath);
                }
                catch (Exception exception)
                {
                    logger.CommandError(CommandKeys.CURRENT_DIRECTORY, exception);
                }
            }

            service.SetVariable(VariableNames.CURRENT_DIRECTORY, currentPath.GetFullPath());
            return new TextResult(service.GetVariable(VariableNames.CURRENT_DIRECTORY));
        }

        private static string ChangePathIfExists(string currentPath, string newPath)
        {
            return Directory.Exists(newPath) ? newPath : currentPath;
        }
    }
}
