using System;
using System.Diagnostics;
using System.IO;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Input;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.View;

namespace Commands.Dir
{
    public class DirCommand : ICommand
    {
        private readonly ICurrentDirectoryService currentDirectory;

        public DirCommand(ICurrentDirectoryService currentDirectory)
        {
            this.currentDirectory = currentDirectory;
        }

        public ICommandResult Execute(CommandContext context)
        {
            string path = GetFullPath(context);
            string cmdStatement = $"dir \"{path}\"";

            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            cmd.StandardInput.WriteLine(cmdStatement);
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();

            cmd.WaitForExit();

            string output = cmd.StandardOutput.ReadToEnd();
            int startIndex = output.IndexOf($">{cmdStatement}{Environment.NewLine}", StringComparison.Ordinal);
            int endIndex = output.LastIndexOf(Environment.NewLine, StringComparison.Ordinal);

            if (startIndex > 0)
            {
                startIndex += 1 + cmdStatement.Length + Environment.NewLine.Length;

                output = endIndex > 0
                    ? output.Substring(startIndex, endIndex - startIndex)
                    : output.Substring(startIndex);
            }

            return new ItemsResult(Item.Plain(output));
        }

        private string GetFullPath(CommandContext context)
        {
            string path = context.GetParameterValue(ParameterKeys.PATH);

            try
            {
                return Path.GetFullPath(path);
            }
            catch
            {
                return currentDirectory.GetCurrentDirectory();
            }
        }

        public static Query BuildConfiguration()
        {
            var command = Query.CreateBuilder();

            var parameter = Parameter.CreateBuilder();
            parameter.Key = ParameterKeys.PATH;
            parameter.ArgumentTemplate = InputRegex.PATH;
            parameter.IsOptional = true;
            parameter.IsRepeatable = true;
            parameter.Documentation.Title = "Path";
            parameter.Documentation.Description = "Specifies drive, directory, and/or files to list.";

            command.Key = "dir";
            command.Representations.AddRange(new[] { "dir", "ls" });
            command.Parameters.Add(parameter.ToImmutable());
            command.Documentation.Title = "Directory";
            command.Documentation.Description = "Displays a list of files and subdirectories in a directory.";

            return command.ToImmutable();
        }
    }
}
