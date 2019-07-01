using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Construction;

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

            // This is too heavy!
            // MSBuildWorkspace msWorkspace = MSBuildWorkspace.Create();
            // Solution solution = msWorkspace.OpenSolutionAsync(solutionPath).Result;

            SolutionFile solution = SolutionFile.Parse(solutionPath);

            foreach (var project in solution.ProjectsInOrder)
            {
                if (project.ProjectType != SolutionProjectType.KnownToBeMSBuildFormat
                    || string.IsNullOrEmpty(project.AbsolutePath))
                {
                    continue;
                }

                yield return project.AbsolutePath;
            }
        }
    }
}
