using System.Collections.Generic;
using BeaverSoft.Texo.Commands.NugetManager.Model;

namespace BeaverSoft.Texo.Commands.NugetManager.Services
{
    public interface IConfigManagementService
    {
        IEnumerable<IConfig> GetAllConfigs();

        IConfig Get(string configPath);

        void LoadFromDirectory(string leafPath);

        void AddOrUpdate(string configPath);

        void Clear();
    }
}
