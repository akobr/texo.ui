using System;
using BeaverSoft.Texo.Commands.NugetManager.Manage;
using BeaverSoft.Texo.Commands.NugetManager.Operations;
using BeaverSoft.Texo.Commands.NugetManager.Services;
using BeaverSoft.Texo.Commands.NugetManager.Stage;
using BeaverSoft.Texo.Core.Commands;
using StrongBeaver.Core;

namespace BeaverSoft.Texo.Commands.NugetManager
{
    public class NugetManagerCommand : ModularCommand, IInitialisable, IDisposable
    {
        private readonly IStageService stage;
        private readonly IManagementService management;

        public NugetManagerCommand(IStageService stage, IManagementService management)
        {
            this.stage = stage ?? throw new ArgumentNullException(nameof(stage));
            this.management = management ?? throw new ArgumentNullException(nameof(management));

            // Stage (default)
            RegisterQuery(StageQueries.STAGE, new StageCommand(stage));

            // Management
            RegisterQuery(ManageQueries.PROJECT, new ProjectCommand(management.Projects));
            RegisterQuery(ManageQueries.PACKAGE, new PackageCommand(management.Packages, management.Projects));

            // Operations
            RegisterQuery(ApplyQueries.INSTALL, new InstallCommand());
            RegisterQuery(ApplyQueries.UNINSTALL, new UninstallCommand());
            RegisterQuery(ApplyQueries.UPDATE, new UpdateCommand());
        }

        // TODO: implement support for initialisable and disposable commands in core
        // start up | shut down
        public void Initialise()
        {
            management.Projects.Initialise();
        }

        public void Dispose()
        {
            management.Projects.Dispose();
        }
    }
}
