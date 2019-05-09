using BeaverSoft.Texo.Core.Actions;
using BeaverSoft.Texo.Core.Path;
using BeaverSoft.Texo.Core.Runtime;
using System.Collections.Generic;

namespace BeaverSoft.Text.Client.VisualStudio.Actions
{
    public class PathOpenAction : IAction
    {
        private readonly EnvDTE80.DTE2 dte;
        private readonly IExecutor executor;

        public PathOpenAction(EnvDTE80.DTE2 dte, IExecutor executor)
        {
            this.dte = dte;
            this.executor = executor;
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

        private void OpenDirectory(string path)
        {
            executor.ProcessAsync($"cd \"{path}\"");
        }

        private void OpenFile(string path, IDictionary<string, string> arguments)
        {
            dte.ItemOperations.OpenFile(path);
        }
    }
}
