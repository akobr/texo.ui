using System.Collections.Immutable;

namespace BeaverSoft.Texo.Commands.FileManager.Stash
{
    public class StashEntry : IStashEntry
    {
        private readonly string lobbyPath;
        private readonly IImmutableList<string> paths;

        public StashEntry(string name, string lobbyPath, IImmutableList<string> paths)
        {
            Name = name;
            this.lobbyPath = lobbyPath;
            this.paths = paths;
        }

        public string Name { get; }

        public string GetLobby()
        {
            return lobbyPath;
        }

        public IImmutableList<string> GetPaths()
        {
            return paths;
        }
    }
}
