using System;
using System.IO;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.Services;

namespace BeaverSoft.Texo.Core.Environment
{
    public class CurrentDirectoryCommand : ICommand
    {
        private const string PARAMETER_PATH = "path";

        private readonly ICurrentDirectoryService service;

        public CurrentDirectoryCommand(ICurrentDirectoryService service)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public ICommandResult Execute(ICommandContext context)
        {
            if (!context.Parameters.TryGetValue(PARAMETER_PATH, out IParameterContext parameter))
            {
                return new TextResult(service.GetCurrentDirectory());
            }

            string currentPath = service.GetCurrentDirectory();

            foreach (string path in parameter.GetValues())
            {
                try
                {
                    if (Path.IsPathRooted(path))
                    {
                        currentPath = ChangePathIfExists(currentPath, path);
                        continue;
                    }

                    string newPath = Path.Combine(currentPath, path);
                    currentPath = ChangePathIfExists(currentPath, newPath);
                }
                catch { /* no operation */ }
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
