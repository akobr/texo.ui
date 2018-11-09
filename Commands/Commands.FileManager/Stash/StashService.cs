using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using BeaverSoft.Texo.Core.Path;
using StrongBeaver.Core.Services.Serialisation;

namespace BeaverSoft.Texo.Commands.FileManager.Stash
{
    public class StashService : IStashService
    {
        private const string STASH_FILE_NAME = "Stashes.json";

        private readonly ISerialisationService serialisation;
        private readonly Dictionary<string, int> nameMap;
        private readonly List<IStashEntry> stashes;

        public StashService(ISerialisationService serialisation)
        {
            this.serialisation = serialisation ?? throw new ArgumentNullException(nameof(serialisation));

            nameMap = new Dictionary<string, int>();
            stashes = new List<IStashEntry>();
        }

        public void Initialise()
        {
            LoadStashes();
        }

        public void Dispose()
        {
            SaveStashes();
        }

        public IImmutableList<IStashEntry> GetStashes()
        {
            return stashes.ToImmutableList();
        }

        public IStashEntry CreateStash(string lobbyPath, IImmutableList<string> paths)
        {
            StashEntry stash = new StashEntry(null, lobbyPath, paths);
            stashes.Insert(0, stash);
            return stash;
        }

        public IStashEntry CreateStash(string lobbyPath, IImmutableList<string> paths, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return CreateStash(lobbyPath, paths);
            }

            if (nameMap.TryGetValue(name, out int existingIndex))
            {
                RemoveStash(existingIndex);
            }

            StashEntry stash = new StashEntry(name, lobbyPath, paths);
            stashes.Insert(0, stash);
            nameMap[name] = 0;

            return stash;
        }

        public IStashEntry GetStash(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            if (!nameMap.TryGetValue(name, out int stashIndex))
            {
                return null;
            }

            return stashes[stashIndex];
        }

        public IStashEntry GetStash(int index)
        {
            if (stashes.Count == 0 && index == 0)
            {
                return null;
            }

            if (index < 0 || index >= stashes.Count)
            {
                throw new IndexOutOfRangeException();
            }

            return stashes[index];
        }

        public void RemoveStash(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            if (!nameMap.TryGetValue(name, out int stashIndex))
            {
                return;
            }

            nameMap.Remove(name);
            stashes.RemoveAt(stashIndex);
        }

        public void RemoveStash(int index)
        {
            if (stashes.Count == 0 && index == 0)
            {
                return;
            }

            if (index < 0 || index >= stashes.Count)
            {
                throw new IndexOutOfRangeException();
            }

            IStashEntry stash = stashes[index];

            if (!string.IsNullOrWhiteSpace(stash.Name))
            {
                nameMap.Remove(stash.Name);
            }

            stashes.RemoveAt(index);
        }

        public void RemoveStash(IStashEntry stash)
        {
            int index = -1;

            if (string.IsNullOrWhiteSpace(stash.Name))
            {
                if (nameMap.TryGetValue(stash.Name, out int nameIndex))
                {
                    index = nameIndex;
                    nameMap.Remove(stash.Name);
                }
            }
            else
            {
                index = stashes.IndexOf(stash);
            }

            if (index < 0)
            {
                return;
            }

            stashes.RemoveAt(index);
        }

        public void Clean()
        {
            nameMap.Clear();
            stashes.Clear();
            DeletePersistentStashes();
        }

        private void LoadStashes()
        {
            string texoDataFolder = PathExtensions.GetTexoDataFolder();

            if (!Directory.Exists(texoDataFolder))
            {
                return;
            }

            string filePath = Path.Combine(texoDataFolder, STASH_FILE_NAME);

            if (!File.Exists(filePath))
            {
                return;
            }

            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                foreach (StashEntry loadedEntry in serialisation.DeserializeFromStream<List<StashEntry>>(file))
                {
                    if (!string.IsNullOrEmpty(loadedEntry.Name))
                    {
                        if (nameMap.ContainsKey(loadedEntry.Name))
                        {
                            continue;
                        }

                        nameMap[loadedEntry.Name] = stashes.Count;
                    }

                    stashes.Add(loadedEntry);
                }
            }
        }

        private void SaveStashes()
        {
            string texoDataFolder = PathExtensions.GetTexoDataFolder();

            if (!Directory.Exists(texoDataFolder))
            {
                Directory.CreateDirectory(texoDataFolder);
            }

            string filePath = Path.Combine(texoDataFolder, STASH_FILE_NAME);

            using (FileStream file = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                serialisation.SerializeToStream(stashes, file);
            }
        }

        private void DeletePersistentStashes()
        {
            string texoDataFolder = PathExtensions.GetTexoDataFolder();

            if (!Directory.Exists(texoDataFolder))
            {
                return;
            }

            string filePath = Path.Combine(texoDataFolder, STASH_FILE_NAME);
            File.Delete(filePath);
        }
    }
}
