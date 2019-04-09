using System.Diagnostics;

namespace BeaverSoft.Texo.Core.Path.Actions
{
    public class SystemOpenStrategy : IOpenFileStrategy, IOpenDirectoryStrategy
    {
        public void Open(string path)
        {
            Process.Start(path);
        }
    }
}
