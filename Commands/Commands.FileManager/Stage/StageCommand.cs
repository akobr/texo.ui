using System;
using System.Collections.Generic;
using System.Text;
using BeaverSoft.Texo.Commands.FileManager.Stash;
using BeaverSoft.Texo.Core.Commands;

namespace BeaverSoft.Texo.Commands.FileManager.Stage
{
    public class StageCommand : InlineIntersectionCommand
    {
        private readonly IStageService stage;
        private readonly IStashService stashes;

        public StageCommand(IStageService stage, IStashService stashes)
        {
            this.stage = stage;
            this.stashes = stashes;

            RegisterQueryMethod(StageQueries.STATUS, Status);
            RegisterQueryMethod(StageQueries.LIST, List);
            RegisterQueryMethod(StageQueries.ADD, Add);
            RegisterQueryMethod(StageQueries.REMOVE, Remove);
        }

        private ICommandResult Status(CommandContext context)
        {
            throw new NotImplementedException();
        }

        private ICommandResult List(CommandContext context)
        {
            throw new NotImplementedException();
        }

        private ICommandResult Add(CommandContext context)
        {
            throw new NotImplementedException();
        }

        private ICommandResult Remove(CommandContext context)
        {
            throw new NotImplementedException();
        }
    }
}
