using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Actions;

namespace BeaverSoft.Texo.Core.Path.Actions
{
    public class OpenPathAction : IAction
    {
        private readonly IOpenFileStrategy fileStrategy;
        private readonly IOpenDirectoryStrategy directoryStrategy;

        public OpenPathAction()
        {
            SystemOpenStrategy systemOpenStrategy = new SystemOpenStrategy();
            fileStrategy = systemOpenStrategy;
            directoryStrategy = systemOpenStrategy;
        }

        public OpenPathAction(IOpenFileStrategy fileStrategy, IOpenDirectoryStrategy directoryStrategy)
        {
            this.fileStrategy = fileStrategy;
            this.directoryStrategy = directoryStrategy;
        }

        public Task ExecuteAsync(IDictionary<string, string> arguments)
        {
            if (!arguments.TryGetValue(ActionParameters.PATH, out string path)
                || string.IsNullOrWhiteSpace(path))
            {
                return Task.CompletedTask;
            }

            if (File.Exists(path))
            {
                return fileStrategy.OpenAsync(path);
            }
            else if(Directory.Exists(path))
            {
                return directoryStrategy.OpenAsync(path);
            }

            return Task.CompletedTask;
        }
    }
}
