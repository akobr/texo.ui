using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using BeaverSoft.Texo.Commands.NugetManager.Model;
using BeaverSoft.Texo.Commands.NugetManager.Processing.Strategies;
using BeaverSoft.Texo.Core.Path;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Commands.NugetManager.Services
{
    public class ConfigManagementService : IConfigManagementService
    {
        private const string NUGET_CONFIG_FILE_NAME = "nuget.config";

        private readonly ILogService logger;
        private readonly ISourceManagementService sources;
        private readonly HashSet<string> processedDirectories;

        private ImmutableSortedDictionary<string, IConfig> configs;

        public ConfigManagementService(
            ISourceManagementService sources,
            ILogService logger)
        {
            this.sources = sources;
            this.logger = logger;
            processedDirectories = new HashSet<string>(new InsensitiveFullPathComparer());

            ResetConfigs();
        }

        public IEnumerable<IConfig> GetAllConfigs()
        {
            return configs.Values;
        }

        public IConfig Get(string configPath)
        {
            string key = configPath.GetFullConsolidatedPath();
            configs.TryGetValue(key, out IConfig config);
            return config;
        }

        public void AddOrUpdate(string configPath)
        {
            IConfig config = BuildConfig(configPath);

            if (config == null)
            {
                return;
            }

            string key = configPath.GetFullConsolidatedPath();
            configs.SetItem(key, config);
        }

        public void LoadFromDirectory(string leafPath)
        {
            DirectoryInfo directory = new DirectoryInfo(leafPath);

            while (directory != null)
            {
                if (!directory.Exists
                    || processedDirectories.Contains(directory.FullName))
                {
                    break;
                }

                processedDirectories.Add(directory.FullName);
                string configFile = directory.FullName.CombinePathWith(NUGET_CONFIG_FILE_NAME);

                if (File.Exists(configFile))
                {
                    AddOrUpdate(configFile);
                }

                directory = Directory.GetParent(directory.FullName);
            }
        }

        public void Clear()
        {
            ResetConfigs();
        }

        private IConfig BuildConfig(string configPath)
        {
            var strategy = new ConfigProcessingStrategy(logger);
            IConfig config = strategy.Process(configPath);

            if (config != null)
            {
                sources.AddRange(config.Sources);
            }

            return config;
        }

        private void ResetConfigs()
        {
            configs = ImmutableSortedDictionary.Create<string, IConfig>(new InsensitiveFullPathComparer());
            processedDirectories.Clear();
        }
    }
}
