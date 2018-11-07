using System;

namespace BeaverSoft.Texo.Commands.NugetManager.Model.Sources
{
    public interface ISource
    {
        string Name { get; }

        Uri Address { get; }
    }
}
