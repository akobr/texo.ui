using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using BeaverSoft.Texo.Commands.NugetManager.Model.Projects;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Commands.NugetManager.Processing.Strategies
{
    public class SolutionProcessingStrategy : ISolutionProcessingStrategy
    {
        private readonly ILogService logger;

        public SolutionProcessingStrategy(ILogService logger)
        {
            this.logger = logger;
        }

        public IImmutableList<IProject> Process(string filePath)
        {
            var result = ImmutableList<IProject>.Empty.ToBuilder();

            foreach (var projectPath in GetProjectPaths(filePath))
            {
                var strategy = new CsharpProjectProcessingStrategy(logger);
                IProject model = strategy.Process(projectPath);

                if (model == null)
                {
                    continue;
                }

                result.Add(model);
            }

            return result.ToImmutable();
        }

        public static IEnumerable<string> GetProjectPaths(string solutionPath)
        {
            if (!File.Exists(solutionPath))
            {
                yield break;
            }

            MSBuildWorkspace msWorkspace = MSBuildWorkspace.Create();
            Solution solution = msWorkspace.OpenSolutionAsync(solutionPath).Result;

            foreach (var project in solution.Projects)
            {
                if (string.IsNullOrEmpty(project.FilePath)
                    || project.Language != "csharp")
                {
                    continue;
                }

                yield return project.FilePath;
            }
        }
    }
}
