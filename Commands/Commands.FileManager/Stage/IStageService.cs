using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using BeaverSoft.Texo.Commands.FileManager.Stash;
using StrongBeaver.Core;

namespace BeaverSoft.Texo.Commands.FileManager.Stage
{
    public interface IStageService : IDisposable, IInitialisable
    {
        IImmutableList<string> GetPaths();

        void Add(IEnumerable<string> newPaths);

        void Add(IStashEntry stash);

        void Remove(IEnumerable<string> newPaths);

        string GetLobby();

        void SetLobby(string path);

        void RemoveLobby();

        void Clear();
    }
}
