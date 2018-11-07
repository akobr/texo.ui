using System;
using BeaverSoft.Texo.Commands.FileManager.Stage;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Path;

namespace BeaverSoft.Texo.Commands.NugetManager.Stage
{
    public class StageCommand : InlineIntersectionCommand
    {
        private readonly IStageService stage;

        public StageCommand(IStageService stage)
        {
            this.stage = stage;

            RegisterQueryMethod(StageQueries.STATUS, Status);
            RegisterQueryMethod(StageQueries.ADD, Add);
            RegisterQueryMethod(StageQueries.REMOVE, Remove);
        }

        private ICommandResult Status(CommandContext context)
        {
            context.GetOption()
        }

        private ICommandResult Add(CommandContext context)
        {
            foreach (string stringPath in context.GetParameterValues(ParameterKeys.PATH))
            {
                TexoPath path = new TexoPath(StringSplitOptions)
            }
        }

        private ICommandResult Remove(CommandContext context)
        {
            throw new NotImplementedException();
        }
    }
}
