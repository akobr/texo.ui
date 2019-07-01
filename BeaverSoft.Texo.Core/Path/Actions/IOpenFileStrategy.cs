using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.Path.Actions
{
    public interface IOpenFileStrategy
    {
        Task OpenAsync(string fileDirectory);
    }
}
