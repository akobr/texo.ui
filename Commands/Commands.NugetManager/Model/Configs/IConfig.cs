using System;
using System.Collections.Immutable;
using BeaverSoft.Texo.Commands.NugetManager.Sources;

namespace BeaverSoft.Texo.Commands.NugetManager.Configs
{
    public interface IConfig
    {
        string Id { get; }

        Uri Path { get; }

        IImmutableList<ISourceInfo> Sources { get; }
    }
}
