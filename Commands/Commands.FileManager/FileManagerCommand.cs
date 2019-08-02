using System;
using BeaverSoft.Texo.Commands.FileManager.Operations;
using BeaverSoft.Texo.Commands.FileManager.Stage;
using BeaverSoft.Texo.Commands.FileManager.Stash;
using BeaverSoft.Texo.Core.Commands;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Commands.FileManager
{
    public partial class FileManagerCommand : ModularCommand
    {
        private readonly IStageService stageService;
        private readonly IStashService stashService;
        private readonly ILogService logger;

        public FileManagerCommand(IStageService stageService, IStashService stashService, ILogService logger)
        {
            this.stageService = stageService ?? throw new ArgumentNullException(nameof(stageService));
            this.stashService = stashService ?? throw new ArgumentNullException(nameof(stashService));
            this.logger = logger;

            RegisterQuery(StageQueries.STAGE, new StageCommand(stageService));
            RegisterQuery(StashQueries.STASH, new StashCommand(stashService, stageService));
            RegisterQuery(ApplyQueries.APPLY, new ApplyCommand(stageService, stashService, logger));
        }
    }
}
