using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using BeaverSoft.Texo.Commands.NugetManager.Model;
using StrongBeaver.Core;

namespace BeaverSoft.Texo.Commands.NugetManager.Services
{
    public interface IProjectManagementService : IInitialisable, IDisposable
    {
        IEnumerable<IProject> GetAll();

        IProject Get(string projectPath);

        void AddOrUpdate(string projectPath);

        void Remove(string projectPath);

        IImmutableList<IProject> Find(string searchTerm);

        IImmutableList<IProject> FindByPackage(string packageId);

        void ReloadAll();

        void Fetch();

        void Clear();
    }
}
