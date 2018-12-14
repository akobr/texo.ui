using BeaverSoft.Texo.Commands.FileManager;
using BeaverSoft.Texo.Commands.NugetManager;
using BeaverSoft.Texo.Core;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Environment;
using Commands.CommandLine;
using Commands.ReferenceCheck;
using StrongBeaver.Core.Container;

namespace BeaverSoft.Texo.Test.Client.Forms.Startup
{
    public static class CommandConfig
    {
        public static void RegisterCommands(this CommandFactory factory, SimpleIoc container)
        {
            factory.Register(CommandKeys.CURRENT_DIRECTORY, container.GetInstance<CurrentDirectoryCommand>);
            factory.Register(CommandKeys.TEXO, container.GetInstance<TexoCommand>);
            factory.Register(ReferenceCheckConstants.REF_CHECK, container.GetInstance<ReferenceCheckCommand>);
            factory.Register("command-line", container.GetInstance<CommandLineCommand>);
            factory.Register("file-manager", container.GetInstance<FileManagerCommand>);
            factory.Register("nuget-manager", container.GetInstance<NugetManagerCommand>);
        }
    }
}
