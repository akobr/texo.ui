using System;
using System.Collections.Immutable;
using StrongBeaver.Core;

namespace BeaverSoft.Texo.Commands.FileManager.Stash
{
    public interface IStashService : IInitialisable, IDisposable
    {
        IImmutableList<IStashEntry> GetStashes();

        IStashEntry CreateStash(string lobbyPath, IImmutableList<string> paths);

        IStashEntry CreateStash(string lobbyPath, IImmutableList<string> paths, string name);

        IStashEntry GetStash(string name);

        IStashEntry GetStash(int index);

        void RemoveStash(string name);

        void RemoveStash(int index);

        void RemoveStash(IStashEntry stash);

        void Clean();
    }
}