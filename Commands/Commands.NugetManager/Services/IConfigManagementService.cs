using BeaverSoft.Texo.Commands.NugetManager.Model.Configs;

namespace BeaverSoft.Texo.Commands.NugetManager.Services
{
    public interface IConfigManagementService
    {
        IConfig Get(string configPath);

        IConfig Add(string configPath);

        void Clear();
    }
}
