using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Result;
using StrongBeaver.Core.Services;

namespace BeaverSoft.Texo.Core.View
{
    public class ClearCommand : ICommand
    {
        private readonly IServiceMessageBus messageBus;

        public ClearCommand(IServiceMessageBus messageBus)
        {
            this.messageBus = messageBus;
        }

        public ICommandResult Execute(CommandContext context)
        {
            messageBus?.Send<IClearViewOutputMessage>(new ClearViewOutputMessage());
            return new TextResult("Output view cleared.");
        }
    }
}
