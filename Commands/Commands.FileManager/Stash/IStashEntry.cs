using System.Collections.Immutable;

namespace BeaverSoft.Texo.Commands.FileManager.Stash
{
    public interface IStashEntry
    {
        string Name { get; }

        IImmutableList<string> Paths { get; }
    }
}