using System.Collections.Immutable;

namespace BeaverSoft.Texo.Commands.FileManager.Stash
{
    public class StashEntry : IStashEntry
    {
        public StashEntry(string name, IImmutableList<string> paths)
        {
            Name = name;
            Paths = paths;
        }

        public string Name { get; }

        public IImmutableList<string> Paths { get; }
    }
}
