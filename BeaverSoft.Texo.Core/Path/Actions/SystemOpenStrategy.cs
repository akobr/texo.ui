using System.Diagnostics;
using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.Path.Actions
{
    public class SystemOpenStrategy : IOpenFileStrategy, IOpenDirectoryStrategy
    {
        public Task OpenAsync(string path)
        {
            Process.Start(path);
            return Task.CompletedTask;
        }
    }
}
