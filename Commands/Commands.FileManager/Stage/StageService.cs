using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using BeaverSoft.Texo.Commands.FileManager.Stash;
using BeaverSoft.Texo.Core.Path;
using StrongBeaver.Core.Services.Serialisation;

namespace BeaverSoft.Texo.Commands.FileManager.Stage
{
    public class StageService : IStageService
    {
        private readonly ISerialisationService serialisation;
        private string lobby;
        private ImmutableSortedSet<string> paths;

        public StageService(ISerialisationService serialisation)
        {
            this.serialisation = serialisation ?? throw new ArgumentNullException(nameof(serialisation));
            EmptyStage();
        }

        public void Initialise()
        {
            LoadStage();
        }

        public void Dispose()
        {
            SaveStage();
        }

        public IImmutableList<string> GetPaths()
        {
            return paths.ToImmutableList();
        }

        public void Add(IEnumerable<string> newPaths)
        {
            foreach (string path in newPaths)
            {
                string fullPath = path.GetFullPath();

                if (!paths.Contains(fullPath))
                {
                    paths = paths.Add(fullPath);
                }
            }
        }

        public void Add(IStashEntry stash)
        {
            if (stash == null)
            {
                return;
            }

            Add(stash.Paths);
        }

        public void Remove(IEnumerable<string> newPaths)
        {
            foreach (string path in newPaths)
            {
                string fullPath = path.GetFullPath();
                paths = paths.Remove(fullPath);
            }
        }

        public string GetLobby()
        {
            return lobby ?? string.Empty;
        }

        public void SetLobby(string path)
        {
            if (string.IsNullOrWhiteSpace(path)
                || !path.IsValidPath()
                || !Directory.Exists(path))
            {
                return;
            }

            lobby = path.GetFullPath();
        }

        public void RemoveLobby()
        {
            lobby = string.Empty;
        }

        public void Clear()
        {
            EmptyStage();
            DeletePersistentStage();
        }

        private void LoadStage()
        {
            string texoDataFolder = PathExtensions.GetAndCreateDataDirectoryPath(FileManagerConstants.STORAGE_DIRECTORY_NAME);
            string filePath = texoDataFolder.CombinePathWith(FileManagerConstants.STORAGE_STAGE_FILE_NAME);

            if (!File.Exists(filePath))
            {
                return;
            }

            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                IStashEntry stage = serialisation.DeserializeFromStream<StashEntry>(file);

                if (stage == null)
                {
                    return;
                }

                lobby = stage.LobbyPath ?? string.Empty;
                var pathsBuilder = ImmutableSortedSet.CreateBuilder(new InsensitiveFullPathComparer());
                pathsBuilder.UnionWith(stage.Paths);
                paths = pathsBuilder.ToImmutable();
            }
        }

        private void SaveStage()
        {
            string texoDataFolder = PathExtensions.GetAndCreateDataDirectoryPath(FileManagerConstants.STORAGE_DIRECTORY_NAME);
            string filePath = texoDataFolder.CombinePathWith(FileManagerConstants.STORAGE_STAGE_FILE_NAME);

            using (FileStream file = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                serialisation.SerializeToStream(BuildStash(), file);
            }
        }

        private void EmptyStage()
        {
            lobby = string.Empty;
            paths = ImmutableSortedSet.Create((IComparer<string>) new InsensitiveFullPathComparer());
        }

        private StashEntry BuildStash()
        {
            return new StashEntry(null, lobby, paths.ToImmutableList());
        }

        private static void DeletePersistentStage()
        {
            string texoDataFolder = PathExtensions.GetAndCreateDataDirectoryPath(FileManagerConstants.STORAGE_DIRECTORY_NAME);
            string filePath = texoDataFolder.CombinePathWith(FileManagerConstants.STORAGE_STAGE_FILE_NAME);
            File.Delete(filePath);
        }
    }
}