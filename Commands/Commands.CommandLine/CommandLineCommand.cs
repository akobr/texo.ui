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
        private const string GIT_BASH_FIRST_PATH = @"c:\Program Files\Git\git-bash.exe";
        private const string GIT_BASH_SECOND_PATH = @"c:\Program Files (x86)\Git\git-bash.exe";

        public ICommandResult Execute(CommandContext context)
        {
            if (context.FirstQuery == "power-shell")
            {
                StartShell("powershell.exe");
            }
            else if (context.FirstQuery == "git-bash")
            {
                if (File.Exists(GIT_BASH_FIRST_PATH))
                {
                    StartShell(GIT_BASH_FIRST_PATH);
                }
                else if (File.Exists(GIT_BASH_SECOND_PATH))
                {
                    StartShell(GIT_BASH_SECOND_PATH);
                }
                else
                {
                    StartShell("cmd.exe");
                }
            }
            else
            {
                StartShell("cmd.exe");
            }

            return new TextResult("Command-line is starting...");
        }

        private static void StartShell(string path)
        {
            Process cmd = new Process
            {
                StartInfo =
                {
                    FileName = path,
                    WorkingDirectory = Environment.CurrentDirectory
                }
            };

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
            powerShell.Representations.AddRange(new [] { "power-shell", "power", "ps", "p" });
            powerShell.Documentation.Title = "Start power shell";
            powerShell.Documentation.Description = "Starts PowerShell in current working directory.";

            var gitBash = Query.CreateBuilder();
            gitBash.Key = "git-bash";
            gitBash.Representations.AddRange(new[] { "git-bash", "gbash", "git", "gb", "g" });
            gitBash.Documentation.Title = "Start git bash";
            gitBash.Documentation.Description = "Starts Git bash in current working directory.";

            command.Queries.Add(powerShell.ToImmutable());
            command.Queries.Add(gitBash.ToImmutable());

            return command.ToImmutable();
        }
    }
}
