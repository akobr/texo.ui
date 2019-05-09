using BeaverSoft.Texo.Core.Actions;
using BeaverSoft.Texo.Core.Path;
using BeaverSoft.Texo.Core.Runtime;
using StrongBeaver.Core.Services.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BeaverSoft.Text.Client.VisualStudio.Actions
{
    public class PathOpenAction : IAction
    {
        private readonly ExtensionContext context;

        public PathOpenAction(ExtensionContext context)
        {
            this.context = context;
        }

        public void Execute(IDictionary<string, string> arguments)
        {
            if (!arguments.TryGetValue(ActionParameters.PATH, out string path))
            {
                return;
            }

            switch (path.GetPathType())
            {
                case PathTypeEnum.File:
                    OpenFile(path, arguments);
                    return;

                case PathTypeEnum.Directory:
                    OpenDirectory(path);
                    return;
            }
        }

        private async void OpenDirectory(string path)
        {
            try
            {
                if (path.Contains(' '))
                {
                    path = $"\"{path}\"";
                }

                await context.Executor.ProcessAsync($"cd {path}");
                await context.Executor.ProcessAsync("dir");
            }
            catch (Exception exception)
            {
                context.Logger.Error("Error during openning directory.", exception);
            }
        }

        private void OpenFile(string path, IDictionary<string, string> arguments)
        {
            if (!context.Threading.IsOnMainThread)
            {
                context.Threading.ExecuteSynchronously(async () => { OpenFile(path, arguments); });
            }

            // TODO: process line
            _ = context.DTE.ItemOperations.OpenFile(path);
        }
    }
}
