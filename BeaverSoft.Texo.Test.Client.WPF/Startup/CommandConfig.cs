using BeaverSoft.Texo.Commands.FileManager;
using BeaverSoft.Texo.Commands.Functions;
using BeaverSoft.Texo.Commands.NugetManager;
using BeaverSoft.Texo.Core;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Help;
using BeaverSoft.Texo.Core.View;
using Commands.Calc;
using Commands.Clipboard;
using Commands.CodeBaseSearch;
using Commands.CommandLine;
using Commands.ReferenceCheck;
using Commands.SpinSport;
using StrongBeaver.Core.Container;

namespace BeaverSoft.Texo.Test.Client.WPF.Startup
{
    public static class CommandConfig
    {
        public static void RegisterCommands(this CommandFactory factory, SimpleIoc container)
        {
            factory.Register(CommandKeys.CURRENT_DIRECTORY, container.GetInstance<CurrentDirectoryCommand>);
            factory.Register(CommandKeys.TEXO, container.GetInstance<TexoCommand>);
            factory.Register(CommandKeys.HELP, container.GetInstance<HelpCommand>);
            factory.Register(CommandKeys.CLEAR, container.GetInstance<ClearCommand>);
            factory.Register(ReferenceCheckConstants.REF_CHECK, container.GetInstance<ReferenceCheckCommand>);
            factory.Register("command-line", container.GetInstance<CommandLineCommand>);
            factory.Register("file-manager", container.GetInstance<FileManagerCommand>);
            factory.Register("nuget-manager", container.GetInstance<NugetManagerCommand>);
            factory.Register("calculator", container.GetInstance<CalcCommand>);
            factory.Register(ClipboardConstants.COMMAND_NAME, container.GetInstance<ClipboardCommand>);
            factory.Register("functions", container.GetInstance<FunctionsCommand>);
            //factory.Register(CodeBaseSearchConstants.COMMAND_NAME, container.GetInstance<CodeBaseSearchCommand>);
            factory.Register(SpinSportConstants.COMMAND_NAME, container.GetInstance<SpinSportCommand>);
        }
    }
}
