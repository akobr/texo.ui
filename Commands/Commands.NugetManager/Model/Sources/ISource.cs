using System;

namespace BeaverSoft.Texo.Commands.NugetManager.Model.Sources
{
    public interface ISource
    {
        Uri Address { get; }
    }
}
