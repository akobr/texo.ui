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

                foreach (string itemPath in GetFilesFromDirectories(path))
                {
                    ProcessFile(itemPath);
                }

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

        private IEnumerable<string> GetFilesFromDirectories(TexoPath path)
        {
            foreach (string directory in path.GetDirectories())
            {
                foreach (string solutionFile in Directory.GetFiles(directory, "*.sln", SearchOption.AllDirectories))
                {
                    yield return solutionFile;
                }

                foreach (string projectFile in Directory.GetFiles(directory, "*.csproj", SearchOption.AllDirectories))
                {
                    yield return projectFile;
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
                case ".sln":
                    var solutionStrategy = new SolutionProcessingStrategy(logger);
                    foreach (IProject project in solutionStrategy.Process(filePath))
                    {
                        projectManagement.AddOrUpdate(project);
                    }
                    break;

                case ".csproj":
                    var projectStrategy = new CsharpProjectProcessingStrategy(logger);
                    projectManagement.AddOrUpdate(projectStrategy.Process(filePath));
                    break;
            }
        }
    }
}
