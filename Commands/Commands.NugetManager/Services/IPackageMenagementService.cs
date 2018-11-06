using System;
using System.Collections.Generic;
using System.Text;
using BeaverSoft.Texo.Commands.NugetManager.Packages;

namespace BeaverSoft.Texo.Commands.NugetManager.Services
{
    public interface IPackageMenagementService
    {
        IPackageInfo Get(string packageId);
    }
}
