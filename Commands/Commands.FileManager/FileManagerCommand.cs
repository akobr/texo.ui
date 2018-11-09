using System;
using BeaverSoft.Texo.Commands.FileManager.Operations;
using BeaverSoft.Texo.Commands.FileManager.Stage;
using BeaverSoft.Texo.Commands.FileManager.Stash;
using BeaverSoft.Texo.Core.Commands;

namespace BeaverSoft.Texo.Commands.FileManager
{
    public partial class FileManagerCommand : IntersectionCommand
    {
        private readonly IStageService stageService;
        private readonly IStashService stashService;

        public FileManagerCommand(IStageService stageService, IStashService stashService)
        {
            this.stageService = stageService;
            this.stashService = stashService;

            RegisterCommand(StageQueries.STAGE, new StageCommand(stageService));
            RegisterCommand(StashQueries.STASH, new StashCommand(stashService, stageService));
            RegisterCommand(ApplyQueries.APPLY, new ApplyCommand());
        }
    }
}
