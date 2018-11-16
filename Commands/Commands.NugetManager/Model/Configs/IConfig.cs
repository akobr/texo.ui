using System.Collections.Immutable;

namespace BeaverSoft.Texo.Commands.NugetManager.Model.Configs
{
    public interface IConfig
    {
        string Path { get; }

        IImmutableList<string> Sources { get; }
    }
}
