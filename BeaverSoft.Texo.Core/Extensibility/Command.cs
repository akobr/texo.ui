using System;
using BeaverSoft.Texo.Core.Commands;

namespace BeaverSoft.Texo.Core.Extensibility
{
    public abstract class Command : ICommand
    {
        public ICommandResult Execute(CommandContext context)
        {
            throw new NotImplementedException();
        }
    }
}
