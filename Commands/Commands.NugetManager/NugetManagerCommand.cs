using System;
using BeaverSoft.Texo.Commands.FileManager.Stage;
using BeaverSoft.Texo.Commands.NugetManager.Manage;
using BeaverSoft.Texo.Commands.NugetManager.Operations;
using BeaverSoft.Texo.Commands.NugetManager.Stage;
using BeaverSoft.Texo.Core.Commands;

namespace BeaverSoft.Texo.Commands.NugetManager
{
    public class NugetManagerCommand : IntersectionCommand
    {
        private readonly IStageService stage;


        public NugetManagerCommand(IStageService stage)
        {
            this.stage = stage ?? throw new ArgumentNullException(nameof(stage));

            // Stage (default)
            RegisterCommand(StageQueries.STAGE, new StageCommand(stage));

            // Management
            RegisterCommand(ManageQueries.PROJECT, new ProjectCommand());
            RegisterCommand(ManageQueries.PACKAGE, new PackageCommand());

            // Operations
            RegisterCommand(ApplyQueries.INSTALL, new InstallCommand());
            RegisterCommand(ApplyQueries.UNINSTALL, new UninstallCommand());
            RegisterCommand(ApplyQueries.UPDATE, new UpdateCommand());
            RegisterCommand(ApplyQueries.CONSOLIDATE, new ConsolidateCommand());
        }
    }
}
