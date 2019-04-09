using System.Collections.Immutable;

namespace BeaverSoft.Texo.Commands.FileManager.Stash
{
    public class StashEntry : IStashEntry
    {
        public StashEntry(string name, string lobbyPath, IImmutableList<string> paths)
        {
            Name = name;
            LobbyPath = lobbyPath;
            Paths = paths;
        }

        public string Name { get; }

        public string LobbyPath { get; }

        public IImmutableList<string> Paths { get; }
    }
}
