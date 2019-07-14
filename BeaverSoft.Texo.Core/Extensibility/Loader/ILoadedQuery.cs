using System;
using BeaverSoft.Texo.Core.Commands;

namespace BeaverSoft.Texo.Core.Extensibility.Loader
{
    public interface ILoadedQuery
    {
        string Key { get; set; }

        Func<CommandContext, ICommandResult> BuildQuery();
    }
}
