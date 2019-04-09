using System;
using System.Diagnostics;
using System.IO;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Result;

namespace Commands.CommandLine
{
    public class CommandLineCommand : ICommand
    {
        private string POWER_SHELL_PATH = @"%SystemRoot%\system32\WindowsPowerShell\v1.0\powershell.exe";

        public ICommandResult Execute(CommandContext context)
        {
            if (context.FirstQuery == "power-shell"
                && File.Exists(POWER_SHELL_PATH))
            {
                StartShell(POWER_SHELL_PATH);
            }
            else
            {
                StartShell("cmd.exe");
            }

            return new TextResult("Command-line is starting...");
        }

        private void StartShell(string path)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = path;
            cmd.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
            //cmd.StartInfo.UseShellExecute = false;
            //cmd.StartInfo.RedirectStandardInput = true;
            //cmd.StartInfo.RedirectStandardOutput = true;
            cmd.Start();
        }

        public static Query BuildConfiguration()
        {
            var command = Query.CreateBuilder();

            command.Key = "command-line";
            command.Representations.AddRange(new[] { "command-line", "cmd" });
            command.Documentation.Title = "Start command line";
            command.Documentation.Description = "Starts command line in current working directory.";

            var powerShell = Query.CreateBuilder();
            powerShell.Key = "power-shell";
            powerShell.Representations.AddRange(new []{ "power-shell", "powershell", "power", "ps"});
            powerShell.Documentation.Title = "Start power shell";
            powerShell.Documentation.Description = "Starts PowerShell in current working directory.";

            command.Queries.Add(powerShell.ToImmutable());

            return command.ToImmutable();
        }
    }
}
