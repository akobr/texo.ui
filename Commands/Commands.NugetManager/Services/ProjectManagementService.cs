using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using BeaverSoft.Texo.Commands.NugetManager.Model;
using BeaverSoft.Texo.Commands.NugetManager.Processing;
using BeaverSoft.Texo.Commands.NugetManager.Processing.Strategies;
using BeaverSoft.Texo.Core.Path;
using StrongBeaver.Core.Services.Logging;
using StrongBeaver.Core.Services.Serialisation;

namespace BeaverSoft.Texo.Commands.NugetManager.Services
{
    public class ProjectManagementService : IProjectManagementService
    {
        private readonly ILogService logger;
        private readonly ISerialisationService serialisation;
        private readonly IConfigManagementService configs;
        private readonly IPackageManagementService packages;

        private ImmutableSortedDictionary<string, IProject> projects;

        public ProjectManagementService(
            IPackageManagementService packages,
            IConfigManagementService configs,
            ISerialisationService serialisation,
            ILogService logger)
        {
            this.packages = packages ?? throw new ArgumentNullException(nameof(packages));
            this.configs = configs ?? throw new ArgumentNullException(nameof(configs));
            this.serialisation = serialisation ?? throw new ArgumentNullException(nameof(serialisation));
            this.logger = logger;
            ResetProjects();
        }

        public void Initialise()
        {
            LoadStageProjects();
        }

        public void Dispose()
        {
            SaveStageProjects();
        }

        public void AddOrUpdate(string projectPath)
        {
            SetProject(projectPath, false);
        }

        public void ReloadAll()
        {
            ReloadAll(false);
        }

        public void Fetch()
        {
            ReloadAll(true);
        }

        public void Clear()
        {
            ResetProjects();
            DeletePersistentStageProjects();
        }

        public IImmutableList<IProject> Find(string searchTerm)
        {
            var currentProjects = projects;
            var result = ImmutableList<IProject>.Empty.ToBuilder();

            foreach (string projectKey in currentProjects.Keys)
            {
                if (projectKey.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    result.Add(currentProjects[projectKey]);
                }
            }

            return result.ToImmutable();
        }

        public IImmutableList<IProject> FindByPackage(string packageId)
        {
            var currentProjects = projects;
            var result = ImmutableList<IProject>.Empty.ToBuilder();

            foreach (IProject project in currentProjects.Values)
            {
                if (project.Packages.ContainsKey(packageId))
                {
                    result.Add(project);
                }
            }

            return result.ToImmutable();
        }

        public IProject Get(string projectPath)
        {
            projects.TryGetValue(projectPath, out IProject result);
            return result;
        }

        public IEnumerable<IProject> GetAll()
        {
            return projects.Values;
        }

        public void Remove(string projectPath)
        {
            if (string.IsNullOrWhiteSpace(projectPath))
            {
                return;
            }

            string key = projectPath.GetFullConsolidatedPath();
            projects = projects.Remove(key);
        }

        private void SetProject(string projectPath, bool fetch)
        {
            IProject project = BuildProject(projectPath, fetch);

            if (project == null)
            {
                return;
            }

            string key = projectPath.GetFullConsolidatedPath();
            projects = projects.SetItem(key, project);
        }

        private void ReloadAll(bool fetch)
        {
            var currentProjects = projects;
            ResetProjects();

            foreach (IProject project in currentProjects.Values)
            {
                SetProject(project.Path, fetch);
            }
        }

        private IProject BuildProject(string filePath, bool fetch)
        {
            IProjectProcessingStrategy projectStrategy = fetch
                ? new CsharpProjectProcessingStrategy(packages, logger)
                : new CsharpProjectProcessingStrategy(new OnlyFetchedPackageSourceStrategy(packages), logger);
            IProject project = projectStrategy.Process(filePath);

            if (project == null)
            {
                return null;
            }

            configs.LoadFromDirectory(filePath.GetParentDirectoryPath());
            return project;
        }

        private IEnumerable<string> GetProjectPaths()
        {
            var currentProjects = projects;
            foreach (IProject project in currentProjects.Values)
            {
                yield return project.Path.GetFullPath();
            }
        }

        private void LoadStageProjects()
        {
            string texoDataFolder = PathExtensions.GetAndCreateDataDirectoryPath(NugetManagerConstants.STORAGE_DIRECTORY_NAME);
            string filePath = texoDataFolder.CombinePathWith(NugetManagerConstants.STORAGE_STAGE_PROJECTS_FILE_NAME);

            if (!File.Exists(filePath))
            {
                return;
            }

            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                IEnumerable<string> projectPaths = serialisation.DeserializeFromStream<List<string>>(file);

                foreach (string projectPath in projectPaths)
                {
                    AddOrUpdate(projectPath);
                }
            }
        }

        private void SaveStageProjects()
        {
            string texoDataFolder = PathExtensions.GetAndCreateDataDirectoryPath(NugetManagerConstants.STORAGE_DIRECTORY_NAME);
            string filePath = texoDataFolder.CombinePathWith(NugetManagerConstants.STORAGE_STAGE_PROJECTS_FILE_NAME);

            using (FileStream file = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                serialisation.SerializeToStream(GetProjectPaths(), file);
            }
        }

        private void ResetProjects()
        {
            projects = ImmutableSortedDictionary.Create<string, IProject>(new InsensitiveFullPathComparer());
            configs.Clear();
        }

        private static void DeletePersistentStageProjects()
        {
            string texoDataFolder = PathExtensions.GetAndCreateDataDirectoryPath(NugetManagerConstants.STORAGE_DIRECTORY_NAME);
            string filePath = texoDataFolder.CombinePathWith(NugetManagerConstants.STORAGE_STAGE_PROJECTS_FILE_NAME);
            File.Delete(filePath);
        }
    }
}