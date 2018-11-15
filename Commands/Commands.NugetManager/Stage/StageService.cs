using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using BeaverSoft.Texo.Commands.NugetManager.Model.Configs;
using BeaverSoft.Texo.Commands.NugetManager.Model.Projects;
using BeaverSoft.Texo.Commands.NugetManager.Model.Sources;
using BeaverSoft.Texo.Commands.NugetManager.Processing.Strategies;
using BeaverSoft.Texo.Commands.NugetManager.Services;
using BeaverSoft.Texo.Core.Path;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Commands.NugetManager.Stage
{
    public class StageService : IStageService
    {
        private const string FILE_EXTENSION_SOLUTION = ".sln";
        private const string FILE_EXTENSION_PROJECT = ".csproj";

        private readonly IProjectManagementService projectManagement;
        private readonly ILogService logger;

        public StageService(
            IProjectManagementService projectManagement,
            ILogService logger)
        {
            this.projectManagement = projectManagement ?? throw new ArgumentNullException(nameof(projectManagement));
            this.logger = logger;
        }

        public IImmutableList<IProject> GetProjects()
        {
            return projectManagement.GetAllProjects().ToImmutableList();
        }

        public IImmutableList<IConfig> GetConfigs()
        {
            throw new NotImplementedException();
        }

        public IImmutableList<ISource> GetSources()
        {
            throw new NotImplementedException();
        }

        public IImmutableList<IProject> Add(IEnumerable<string> paths)
        {
            foreach (string stringPath in paths)
            {
                TexoPath path = new TexoPath(stringPath);
                ProcessDirectories(path);

                foreach (string filePath in path.GetFiles())
                {
                    ProcessFile(filePath);
                }
            }
        }

        public IImmutableList<IProject> Remove(IEnumerable<string> paths)
        {
            throw new NotImplementedException();
        }

        public void Fetch()
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        private void ProcessDirectories(TexoPath path)
        {
            foreach (string directory in path.GetTopDirectories())
            {
                foreach (string solutionFile in TexoDirectory.GetFiles(directory, PathConstants.ANY_PATH_WILDCARD + FILE_EXTENSION_SOLUTION, SearchOption.AllDirectories))
                {
                    ProcessSolution(solutionFile);
                }

                foreach (string projectFile in TexoDirectory.GetFiles(directory, PathConstants.ANY_PATH_WILDCARD + FILE_EXTENSION_PROJECT, SearchOption.AllDirectories))
                {
                    ProcessCSharpProject(projectFile);
                }
            }
        }

        private void ProcessFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return;
            }

            switch (Path.GetExtension(filePath))
            {
                case FILE_EXTENSION_SOLUTION:
                    ProcessSolution(filePath);
                    break;

                case FILE_EXTENSION_PROJECT:
                    ProcessCSharpProject(filePath);
                    break;
            }
        }

        private void ProcessCSharpProject(string filePath)
        {
            var projectStrategy = new CsharpProjectProcessingStrategy(logger);
            projectManagement.AddOrUpdate(projectStrategy.Process(filePath));
        }

        private void ProcessSolution(string filePath)
        {
            var solutionStrategy = new SolutionProcessingStrategy(logger);
            foreach (IProject project in solutionStrategy.Process(filePath))
            {
                projectManagement.AddOrUpdate(project);
            }
        }
    }
}
