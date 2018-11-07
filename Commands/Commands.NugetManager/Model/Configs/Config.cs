using System;
using System.Collections.Immutable;
using BeaverSoft.Texo.Commands.NugetManager.Extenssions;
using BeaverSoft.Texo.Commands.NugetManager.Model.Sources;

namespace BeaverSoft.Texo.Commands.NugetManager.Model.Configs
{
    class Config : IConfig
    {
        public Config(Uri path, IImmutableList<ISourceInfo> sources)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Sources = sources ?? throw new ArgumentNullException(nameof(sources));
            Id = path.AbsolutePath.GetPathId();
        }

        public string Id { get; }

        public Uri Path { get; }

        public IImmutableList<ISourceInfo> Sources { get; }
    }
}
