using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace BeaverSoft.Texo.Commands.NugetManager.Processing.Strategies
{
    public class SolutionProcessingStrategy : ISolutionProcessingStrategy
    {
        public IEnumerable<string> Process(string filePath)
        {
            return GetProjectPaths(filePath);
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
