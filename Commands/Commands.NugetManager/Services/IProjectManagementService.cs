using System;
using System.Collections.Generic;
using System.Text;
using BeaverSoft.Texo.Commands.NugetManager.Projects;

namespace BeaverSoft.Texo.Commands.NugetManager.Services
{
    public interface IProjectManagementService
    {
        IProject Get(string projectPath);
    }
}
