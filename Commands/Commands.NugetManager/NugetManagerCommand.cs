﻿using System;
using BeaverSoft.Texo.Commands.FileManager.Stage;
using BeaverSoft.Texo.Commands.NugetManager.Manage;
using BeaverSoft.Texo.Commands.NugetManager.Operations;
using BeaverSoft.Texo.Commands.NugetManager.Services;
using BeaverSoft.Texo.Commands.NugetManager.Stage;
using BeaverSoft.Texo.Core.Commands;
using StrongBeaver.Core;

namespace BeaverSoft.Texo.Commands.NugetManager
{
    public class NugetManagerCommand : IntersectionCommand, IInitialisable, IDisposable
    {
        private readonly IStageService stage;
        private readonly IManagementService management;

        public NugetManagerCommand(IStageService stage, IManagementService management)
        {
            this.stage = stage ?? throw new ArgumentNullException(nameof(stage));
            this.management = management ?? throw new ArgumentNullException(nameof(management));

            // Stage (default)
            RegisterCommand(StageQueries.STAGE, new StageCommand(stage));

            // Management
            RegisterCommand(ManageQueries.PROJECT, new ProjectCommand(management.Projects, management.Packages));
            RegisterCommand(ManageQueries.PACKAGE, new PackageCommand(management.Packages, management.Projects));

            // Operations
            RegisterCommand(ApplyQueries.INSTALL, new InstallCommand());
            RegisterCommand(ApplyQueries.UNINSTALL, new UninstallCommand());
            RegisterCommand(ApplyQueries.UPDATE, new UpdateCommand());
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