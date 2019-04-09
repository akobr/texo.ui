using System.Collections.Immutable;

namespace BeaverSoft.Texo.Commands.NugetManager.Model
{
    public interface IConfig
    {
        string Path { get; }

        IImmutableList<string> Sources { get; }
    }
}
