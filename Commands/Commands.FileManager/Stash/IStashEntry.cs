using System.Collections.Immutable;
using BeaverSoft.Texo.Commands.FileManager.Operations;

namespace BeaverSoft.Texo.Commands.FileManager.Stash
{
    public interface IStashEntry : IOperationSource
    {
        string Name { get; }
    }
}