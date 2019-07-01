using BeaverSoft.Texo.Core.Actions;
using BeaverSoft.Texo.Core.Path;
using BeaverSoft.Texo.Core.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeaverSoft.Texo.Test.Client.WPF.Actions
{
    public class PathOpenAction : IAction
    {
        private readonly IExecutor executor;

        public PathOpenAction(IExecutor executor)
        {
            this.executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public async Task ExecuteAsync(IDictionary<string, string> arguments)
        {
            if (!arguments.TryGetValue(ActionParameters.PATH, out string path))
            {
                return;
            }

            switch (path.GetPathType())
            {
                case PathTypeEnum.File:
                    OpenFile(path, arguments);
                    break;

                case PathTypeEnum.Directory:
                    await OpenDirectoryAsync(path);
                    break;
            }
        }

        private async Task OpenDirectoryAsync(string path)
        {           
            if (path.Contains(' '))
            {
                path = $"\"{path}\"";
            }

            // Process.Start(path);
            await executor.ProcessAsync($"cd {path}");
            await executor.ProcessAsync("dir");
        }

        private void OpenFile(string path, IDictionary<string, string> arguments)
        {
            StringBuilder openUrlBuilder = new StringBuilder();
            openUrlBuilder.Append($"vscode://file/{Uri.EscapeDataString(path)}");

            if (arguments.TryGetValue("line", out string line))
            {
                openUrlBuilder.Append($":{line}");
            }

            if (arguments.TryGetValue("column", out string column))
            {
                openUrlBuilder.Append($":{column}");
            }

            Process.Start(openUrlBuilder.ToString());
        }
    }
}
