using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using BeaverSoft.Texo.Commands.FileManager.Operations;
using BeaverSoft.Texo.Commands.FileManager.Stash;
using BeaverSoft.Texo.Core.Path;
using StrongBeaver.Core;

namespace BeaverSoft.Texo.Commands.FileManager.Stage
{
    public interface IStageService : IOperationSource, IDisposable, IInitialisable
    {
        void Add(IEnumerable<string> newPaths);

        void Add(IStashEntry stash);

        void Remove(IEnumerable<string> newPaths);

        void Remove(TexoPath toRemove);

        void SetLobby(string path);

        void RemoveLobby();

        void Clear();
    }
}
