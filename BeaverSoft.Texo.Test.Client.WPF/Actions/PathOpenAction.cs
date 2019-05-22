using BeaverSoft.Texo.Core.Actions;
using BeaverSoft.Texo.Core.Path;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace BeaverSoft.Texo.Test.Client.WPF.Actions
{
    public class PathOpenAction : IAction
    {
        public Task ExecuteAsync(IDictionary<string, string> arguments)
        {
            if (!arguments.TryGetValue(ActionParameters.PATH, out string path))
            {
                return Task.CompletedTask;
            }

            switch (path.GetPathType())
            {
                case PathTypeEnum.File:
                    OpenFile(path, arguments);
                    break;

                case PathTypeEnum.Directory:
                    OpenDirectory(path);
                    break;
            }

            return Task.CompletedTask;
        }

        private void OpenDirectory(string path)
        {
            Process.Start(path);
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
