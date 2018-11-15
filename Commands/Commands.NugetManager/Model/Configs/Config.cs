using System;
using System.Collections.Immutable;
using BeaverSoft.Texo.Commands.NugetManager.Model.Sources;
using BeaverSoft.Texo.Core.Path;

namespace BeaverSoft.Texo.Commands.NugetManager.Model.Configs
{
    class Config : IConfig
    {
        public Config()
        {
            // no operation
        }

        public Config(string path, IImmutableList<ISourceInfo> sources)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Sources = sources ?? throw new ArgumentNullException(nameof(sources));
        }

        public string Path { get; set; }

        public IImmutableList<ISourceInfo> Sources { get; set; }
    }
}
