using System.Collections.Generic;
using System.Collections.Immutable;
using BeaverSoft.Texo.Commands.FileManager.Stash;

namespace BeaverSoft.Texo.Commands.FileManager.Stage
{
    public interface IStageService
    {
        IImmutableList<string> GetPaths();

        void Add(IEnumerable<string> paths);

        void Add(IStashEntry stash);

        void Remove(IEnumerable<string> paths);

        void Clear();
    }
}
