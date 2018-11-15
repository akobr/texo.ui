using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using BeaverSoft.Texo.Commands.NugetManager.Model.Projects;
using BeaverSoft.Texo.Core.Path;
using StrongBeaver.Core.Services.Serialisation;

namespace BeaverSoft.Texo.Commands.NugetManager.Services
{
    public class ProjectManagementService : IProjectManagementService
    {
        private readonly ISerialisationService serialisation;
        private ImmutableSortedDictionary<string, IProject> projects;

        public ProjectManagementService(ISerialisationService serialisation)
        {
            this.serialisation = serialisation ?? throw new ArgumentNullException(nameof(serialisation));
            ResetProjects();
        }

        public ProjectManagementService()
        {
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
        
        public void AddOrUpdate(IProject project)
        {
            string key = project.Path.GetFullConsolidatedPath();
            projects.SetItem(key, project);
        }

        public void Clear()
        {
            ResetProjects();
            DeletePersistentStageProjects();
        }

        public IImmutableList<IProject> FindProjects(string searchTerm)
        {
            var currentProjects = projects;
            var result = ImmutableList<IProject>.Empty.ToBuilder();

            foreach (string projectKey in currentProjects.Keys)
            {
                if (projectKey.Contains(searchTerm))
                {
                    result.Add(currentProjects[projectKey]);
                }
            }

            return result.ToImmutable();
        }

        public IImmutableList<IProject> FindProjectsByPackage(string packageId)
        {
            var currentProjects = projects;
            var result = ImmutableList<IProject>.Empty.ToBuilder();

            foreach (IProject project in currentProjects.Values)
            {
                if(project.Packages.ContainsKey(packageId))
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

        public IEnumerable<IProject> GetAllProjects()
        {
            return projects.Values;
        }

        public void Remove(IProject project)
        {
            if (project == null)
            {
                return;
            }

            Remove(project.Path);
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
                List<Project> loadedProjects = serialisation.DeserializeFromStream<List<Project>>(file);

                foreach (IProject project in loadedProjects)
                {
                    AddOrUpdate(project);
                }
            }
        }

        private void SaveStageProjects()
        {
            string texoDataFolder = PathExtensions.GetAndCreateDataDirectoryPath(NugetManagerConstants.STORAGE_DIRECTORY_NAME);
            string filePath = texoDataFolder.CombinePathWith(NugetManagerConstants.STORAGE_STAGE_PROJECTS_FILE_NAME);

            using (FileStream file = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                serialisation.SerializeToStream(projects.Values, file);
            }
        }

        private void ResetProjects()
        {
            projects = ImmutableSortedDictionary.Create<string, IProject>(new InsensitiveFullPathComparer());
        }

        private static void DeletePersistentStageProjects()
        {
            string texoDataFolder = PathExtensions.GetAndCreateDataDirectoryPath(NugetManagerConstants.STORAGE_DIRECTORY_NAME);
            string filePath = texoDataFolder.CombinePathWith(NugetManagerConstants.STORAGE_STAGE_PROJECTS_FILE_NAME);
            File.Delete(filePath);
        }
    }
}
