using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using BeaverSoft.Texo.Commands.NugetManager.Model;
using BeaverSoft.Texo.Commands.NugetManager.Processing.Strategies;
using BeaverSoft.Texo.Commands.NugetManager.Services;
using BeaverSoft.Texo.Core.Path;

namespace BeaverSoft.Texo.Commands.NugetManager.Stage
{
    public class StageService : IStageService
    {
        private const string FILE_EXTENSION_SOLUTION = ".sln";
        private const string FILE_EXTENSION_PROJECT = ".csproj";

        private readonly IManagementService management;

        public StageService(IManagementService management)
        {
            this.management = management ?? throw new ArgumentNullException(nameof(management));
        }

        public IImmutableList<IProject> GetProjects()
        {
            return management.Projects.GetAll().ToImmutableList();
        }

        public IImmutableList<IConfig> GetConfigs()
        {
            return management.Configs.GetAllConfigs().ToImmutableList();
        }

        public IImmutableList<string> GetSources()
        {
            return management.Sources.GetAllSources().ToImmutableList();
        }

        public void Add(IEnumerable<string> paths)
        {
            ProcessTexoPaths(paths, AddCSharpProject);
        }

        public void Remove(IEnumerable<string> paths)
        {
            ProcessTexoPaths(paths, RemoveProject);
        }

        public void Fetch()
        {
            management.Projects.Fetch();
        }

        public void Clear()
        {
            management.Projects.Clear();
            management.Packages.Clear();
            management.Configs.Clear();
            management.Sources.Clear();
        }

        private void AddCSharpProject(string filePath)
        {
            management.Projects.AddOrUpdate(filePath);
        }

        private void RemoveProject(string filePath)
        {
            management.Projects.Remove(filePath);
        }

        private static void ProcessTexoPaths(IEnumerable<string> paths, Action<string> projectAction)
        {
            ISet<string> visitedFiles = new HashSet<string>(new InsensitiveFullPathComparer());

            foreach (string stringPath in paths)
            {
                TexoPath path = new TexoPath(stringPath);
                ProcessDirectories(path, projectAction, visitedFiles);

                foreach (string filePath in path.GetFiles())
                {
                    ProcessFile(filePath, projectAction, visitedFiles);
                }
            }
        }

        private static void ProcessDirectories(TexoPath path, Action<string> projectAction, ISet<string> visitedFiles)
        {
            foreach (string directory in path.GetDirectories())
            {
                foreach (string solutionFile in TexoDirectory.GetFiles(directory, PathConstants.ANY_PATH_WILDCARD + FILE_EXTENSION_SOLUTION, SearchOption.AllDirectories))
                {
                    ProcessSolution(solutionFile, projectAction, visitedFiles);
                }

                foreach (string projectFile in TexoDirectory.GetFiles(directory, PathConstants.ANY_PATH_WILDCARD + FILE_EXTENSION_PROJECT, SearchOption.AllDirectories))
                {
                    ProcessProjectFile(projectFile, projectAction, visitedFiles);
                }
            }
        }

        private static void ProcessFile(string filePath, Action<string> projectAction, ISet<string> visitedFiles)
        {
            if (!File.Exists(filePath))
            {
                return;
            }

            switch (Path.GetExtension(filePath))
            {
                case FILE_EXTENSION_SOLUTION:
                    ProcessSolution(filePath, projectAction, visitedFiles);
                    break;

                case FILE_EXTENSION_PROJECT:
                    ProcessProjectFile(filePath, projectAction, visitedFiles);
                    break;
            }
        }

        private static void ProcessProjectFile(string filePath, Action<string> projectAction, ISet<string> visitedFiles)
        {
            if (visitedFiles.Contains(filePath))
            {
                return;
            }

            visitedFiles.Add(filePath);
            projectAction(filePath);
        }

        private static void ProcessSolution(string filePath, Action<string> projectAction, ISet<string> visitedFiles)
        {
            if (visitedFiles.Contains(filePath))
            {
                return;
            }

            visitedFiles.Add(filePath);
            var solutionStrategy = new SolutionProcessingStrategy();
            foreach (string projectPath in solutionStrategy.Process(filePath))
            {
                ProcessProjectFile(projectPath, projectAction, visitedFiles);
            }
        }
    }
}
