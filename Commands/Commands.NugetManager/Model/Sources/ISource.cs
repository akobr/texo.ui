using System;
using System.Collections.Immutable;
using BeaverSoft.Texo.Commands.NugetManager.Packages;

namespace BeaverSoft.Texo.Commands.NugetManager.Sources
{
    public interface ISource
    {
        string Name { get; }

        Uri Address { get; }
    }
}
