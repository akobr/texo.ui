using System.Collections.Generic;
using System.Collections.Immutable;
using BeaverSoft.Texo.Commands.NugetManager.Model.Projects;

namespace BeaverSoft.Texo.Commands.NugetManager.Stage
{
    public interface IStageService
    {
        IImmutableList<IProject> GetPaths();

        void Add(IEnumerable<IProject> projects);

        void Remove(IEnumerable<string> projectPaths);

        void Clear();
    }
}
