using System;
using System.IO;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Extensions;
using BeaverSoft.Texo.Core.Result;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Core.Environment
{
    public class CurrentDirectoryCommand : ICommand
    {
        private const string PARAMETER_PATH = "path";

        private readonly ICurrentDirectoryService service;
        private readonly ILogService logger;

        public CurrentDirectoryCommand(ICurrentDirectoryService service, ILogService logger)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
            this.logger = logger;
        }

        public ICommandResult Execute(CommandContext context)
        {
            if (!context.Parameters.TryGetValue(PARAMETER_PATH, out ParameterContext parameter))
            {
                return new TextResult(service.GetCurrentDirectory());
            }

            string currentPath = service.GetCurrentDirectory();

            foreach (string path in parameter.GetValues())
            {
                try
                {
                    // TODO: Even rooted path can be relative
                    if (Path.IsPathRooted(path))
                    {
                        currentPath = ChangePathIfExists(currentPath, path);
                        continue;
                    }

                    string newPath = Path.Combine(currentPath, path);
                    currentPath = ChangePathIfExists(currentPath, newPath);
                }
                catch (Exception exception)
                {
                    logger.CommandError(CommandKeys.CURRENT_DIRECTORY, exception);
                }
            }

            service.SetCurrentDirectory(currentPath);
            return new TextResult(service.GetCurrentDirectory());
        }

        private static string ChangePathIfExists(string currentPath, string newPath)
        {
            return Directory.Exists(newPath) ? newPath : currentPath;
        }
    }
}
