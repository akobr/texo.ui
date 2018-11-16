using System.Collections.Generic;
using BeaverSoft.Texo.Commands.NugetManager.Model.Configs;

namespace BeaverSoft.Texo.Commands.NugetManager.Services
{
    public interface IConfigManagementService
    {
        IEnumerable<IConfig> GetAllConfigs();

        IConfig Get(string configPath);

        void AddOrUpdate(string configPath);

        void Clear();
    }
}
