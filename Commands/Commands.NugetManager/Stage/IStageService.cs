using System.Collections.Generic;
using System.Collections.Immutable;
using BeaverSoft.Texo.Commands.NugetManager.Model;

namespace BeaverSoft.Texo.Commands.NugetManager.Stage
{
    public interface IStageService
    {
        IImmutableList<IProject> GetProjects();

        IImmutableList<IConfig> GetConfigs();

        IImmutableList<string> GetSources();

        void Add(IEnumerable<string> paths);

        void Remove(IEnumerable<string> paths);

        void Fetch();

        void Clear();
    }
}
