using System.Collections.Immutable;

namespace BeaverSoft.Texo.Commands.FileManager.Stash
{
    public class StashEntry
    {
        public string Name { get; set; }

        public ImmutableList<string> Paths { get; set; }
    }
}
