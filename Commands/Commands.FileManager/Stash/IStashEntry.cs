using System.Collections.Immutable;

namespace BeaverSoft.Texo.Commands.FileManager.Stash
{
    public interface IStashEntry
    {
        string Name { get; }

        string LobbyPath { get; }

        IImmutableList<string> Paths { get; }
    }
}