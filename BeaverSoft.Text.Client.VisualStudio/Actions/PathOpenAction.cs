using BeaverSoft.Texo.Core.Actions;
using BeaverSoft.Texo.Core.Path;
using EnvDTE;
using StrongBeaver.Core.Services.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeaverSoft.Text.Client.VisualStudio.Actions
{
    public class PathOpenAction : IAction
    {
        private readonly ExtensionContext context;

        public PathOpenAction(ExtensionContext context)
        {
            this.context = context;
        }

        public async Task ExecuteAsync(IDictionary<string, string> arguments)
        {
            if (!arguments.TryGetValue(ActionParameters.PATH, out string path))
            {
                return;
            }

            try
            {
                switch (path.GetPathType())
                {
                    case PathTypeEnum.File:
                        await OpenFileAsync(path, arguments);
                        return;

                    case PathTypeEnum.Directory:
                        await OpenDirectoryAsync(path);
                        return;
                }
            }
            catch (Exception exception)
            {
                context.Logger.Error("Error during openning path.", exception);
            }
        }

        private async Task OpenDirectoryAsync(string path)
        {
            if (path.Contains(' '))
            {
                path = $"\"{path}\"";
            }

            await context.Executor.ProcessAsync($"cd {path}");
            await context.Executor.ProcessAsync("dir");
        }

        private async Task OpenFileAsync(string path, IDictionary<string, string> arguments)
        {
            await context.TaskFactory.SwitchToMainThreadAsync();
            
            var window = context.DTE.ItemOperations.OpenFile(path);
            var selection = (TextSelection)window.Document.Selection;

            if (arguments.TryGetValue("line", out string lineText)
                && int.TryParse(lineText, out int line))
            {
                selection.GotoLine(line);
            }

            if (arguments.TryGetValue("column", out string columnText)
                && int.TryParse(columnText, out int column))
            {
                selection.MoveToAbsoluteOffset(column);
            }
        }
    }
}
