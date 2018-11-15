using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using BeaverSoft.Texo.Commands.NugetManager.Model.Configs;
using BeaverSoft.Texo.Core.Path;

namespace BeaverSoft.Texo.Commands.NugetManager.Services
{
    public class ConfigManagementService : IConfigManagementService
    {
        ImmutableSortedDictionary<string, IConfig> configs;

        public ConfigManagementService()
        {
            ResetConfigs();
        }

        public IConfig Add(string configPath)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            ResetConfigs();
        }

        public IConfig Get(string configPath)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IConfig> GetAllConfigs()
        {
            throw new NotImplementedException();
        }

        private void ResetConfigs()
        {
            configs = ImmutableSortedDictionary.Create<string, IConfig>(new InsensitiveFullPathComparer());
        }
    }
}
