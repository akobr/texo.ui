using System;
using BeaverSoft.Texo.Commands.FileManager.Stash;
using BeaverSoft.Texo.Core.Commands;

namespace BeaverSoft.Texo.Commands.FileManager
{
    public partial class FileManagerCommand : IntersectionCommand
    {
        public FileManagerCommand()
        {
            RegisterCommand(StashQueries.STASH, new FileManagerStashCommand());
            RegisterCommand()
        }
    }
}
