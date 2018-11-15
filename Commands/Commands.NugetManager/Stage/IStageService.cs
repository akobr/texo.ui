using System.Collections.Generic;
using System.Collections.Immutable;
using BeaverSoft.Texo.Commands.NugetManager.Model.Configs;
using BeaverSoft.Texo.Commands.NugetManager.Model.Projects;
using BeaverSoft.Texo.Commands.NugetManager.Model.Sources;

namespace BeaverSoft.Texo.Commands.NugetManager.Stage
{
    public interface IStageService
    {
        IImmutableList<IProject> GetProjects();

        IImmutableList<IConfig> GetConfigs();

        IImmutableList<ISource> GetSources();

        void Add(IEnumerable<string> paths);

        void Remove(IEnumerable<string> paths);

        void Fetch();

        void Clear();
    }
}
