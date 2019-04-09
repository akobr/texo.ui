using System;
using BeaverSoft.Texo.Commands.FileManager.Operations;
using BeaverSoft.Texo.Commands.FileManager.Stage;
using BeaverSoft.Texo.Commands.FileManager.Stash;
using BeaverSoft.Texo.Core.Commands;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Commands.FileManager
{
    public partial class FileManagerCommand : IntersectionCommand
    {
        private readonly IStageService stageService;
        private readonly IStashService stashService;
        private readonly ILogService logger;

        public FileManagerCommand(IStageService stageService, IStashService stashService, ILogService logger)
        {
            this.stageService = stageService ?? throw new ArgumentNullException(nameof(stageService));
            this.stashService = stashService ?? throw new ArgumentNullException(nameof(stashService));
            this.logger = logger;

            RegisterCommand(StageQueries.STAGE, new StageCommand(stageService));
            RegisterCommand(StashQueries.STASH, new StashCommand(stashService, stageService));
            RegisterCommand(ApplyQueries.APPLY, new ApplyCommand(stageService, logger));
        }
    }
}
