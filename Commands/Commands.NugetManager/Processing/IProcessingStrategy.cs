using System.Collections.Immutable;
using BeaverSoft.Texo.Commands.NugetManager.Model.Configs;
using BeaverSoft.Texo.Commands.NugetManager.Model.Projects;

namespace BeaverSoft.Texo.Commands.NugetManager.Processing
{
    public interface IProcessingStrategy<TResult>
    {
        TResult Process(string filePath);
    }

    public interface IConfigProcessingStrategy : IProcessingStrategy<IConfig>
    {
        // no member
    }

    public interface IProjectProcessingStrategy : IProcessingStrategy<IProject>
    {
        // no member
    }

    public interface ISolutionProcessingStrategy : IProcessingStrategy<IImmutableList<IProject>>
    {
        // no member
    }
}
