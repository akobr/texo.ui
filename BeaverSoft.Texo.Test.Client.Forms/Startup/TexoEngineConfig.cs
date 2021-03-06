﻿using BeaverSoft.Texo.Commands.FileManager;
using BeaverSoft.Texo.Commands.NugetManager;
using BeaverSoft.Texo.Core;
using Commands.CommandLine;
using Commands.ReferenceCheck;

namespace BeaverSoft.Texo.Test.Client.Forms.Startup
{
    public static class TexoEngineConfig
    {
        public static void InitialiseWithCommands(this TexoEngine engine)
        {
            engine.Initialise(
                ReferenceCheckCommand.BuildConfiguration(),
                CommandLineCommand.BuildConfiguration(),
                FileManagerBuilder.BuildCommand(),
                NugetManagerBuilder.BuildCommand());
        }
    }
}
