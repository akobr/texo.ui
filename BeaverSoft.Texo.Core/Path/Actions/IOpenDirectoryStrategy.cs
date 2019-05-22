using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.Path.Actions
{
    public interface IOpenDirectoryStrategy
    {
        Task OpenAsync(string directoryPath);
    }
}
