using System.Collections.Generic;
using System.Collections.Immutable;
using BeaverSoft.Texo.Commands.NugetManager.Model.Configs;
using BeaverSoft.Texo.Commands.NugetManager.Processing.Strategies;
using BeaverSoft.Texo.Core.Path;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Commands.NugetManager.Services
{
    public class ConfigManagementService : IConfigManagementService
    {
        private readonly ILogService logger;
        private ImmutableSortedDictionary<string, IConfig> configs;

        public ConfigManagementService(ILogService logger)
        {
            this.logger = logger;
            ResetConfigs();
        }

        public IEnumerable<IConfig> GetAllConfigs()
        {
            return configs.Values;
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

        public void Clear()
        {
            ResetConfigs();
        }

        public IConfig Get(string configPath)
        {
            string key = configPath.GetFullConsolidatedPath();
            configs.TryGetValue(key, out IConfig config);
            return config;
        }

        private IConfig BuildConfig(string configPath)
        {
            var strategy = new ConfigProcessingStrategy(logger);
            return strategy.Process(configPath);
        }

        private void ResetConfigs()
        {
            configs = ImmutableSortedDictionary.Create<string, IConfig>(new InsensitiveFullPathComparer());
        }
    }
}
