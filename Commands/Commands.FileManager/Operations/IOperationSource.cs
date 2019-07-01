using System.Collections.Immutable;

namespace BeaverSoft.Texo.Commands.FileManager.Operations
{
    public interface IOperationSource
    {
        IImmutableList<string> GetPaths();

        string GetLobby();
    }
}
