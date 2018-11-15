using System.Collections.Immutable;
using BeaverSoft.Texo.Commands.NugetManager.Model.Sources;

namespace BeaverSoft.Texo.Commands.NugetManager.Model.Configs
{
    public interface IConfig
    {
        string Path { get; }

        IImmutableList<ISourceInfo> Sources { get; }
    }
}
