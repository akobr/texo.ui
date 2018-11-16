using System;
using System.Collections.Immutable;

namespace BeaverSoft.Texo.Commands.NugetManager.Model.Configs
{
    class Config : IConfig
    {
        public Config()
        {
            // no operation
        }

        public Config(string path, IImmutableList<string> sources)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Sources = sources ?? throw new ArgumentNullException(nameof(sources));
        }

        public string Path { get; set; }

        public IImmutableList<string> Sources { get; set; }
    }
}
