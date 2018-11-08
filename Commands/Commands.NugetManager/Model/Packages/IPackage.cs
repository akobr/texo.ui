using System;

namespace BeaverSoft.Texo.Commands.NugetManager.Model.Packages
{
    public interface IPackage
    {
        string Id { get; }

        string FullVersion { get; }
        
        Version Version { get; }
    }
}
