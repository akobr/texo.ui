using System.Threading.Tasks;
using BeaverSoft.Texo.Commands.FileManager;
using BeaverSoft.Texo.Commands.Functions;
using BeaverSoft.Texo.Commands.NugetManager;
using BeaverSoft.Texo.Core;
using Commands.Calc;
using Commands.Clipboard;
using Commands.CommandLine;
using Commands.ReferenceCheck;

namespace BeaverSoft.Text.Client.VisualStudio.Startup
{
    public static class TexoEngineConfig
    {
        public static Task InitialiseWithCommandsAsync(this TexoEngine engine)
        {
            return engine.InitialiseAsync(
                ReferenceCheckCommand.BuildConfiguration(),
                CommandLineCommand.BuildConfiguration(),
                FileManagerBuilder.BuildCommand(),
                NugetManagerBuilder.BuildCommand(),
                CalcCommand.BuildConfiguration(),
                ClipboardBuilder.BuildCommand(),
                FunctionsBuilder.BuildCommand());
        }
    }
}
