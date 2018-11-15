using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using BeaverSoft.Texo.Commands.NugetManager.Model.Projects;
using StrongBeaver.Core;

namespace BeaverSoft.Texo.Commands.NugetManager.Services
{
    public interface IProjectManagementService : IInitialisable, IDisposable
    {
        IEnumerable<IProject> GetAllProjects();

        IProject Get(string projectPath);

        void AddOrUpdate(IProject project);

        void Remove(IProject project);

        void Remove(string projectPath);

        IImmutableList<IProject> FindProjects(string searchTerm);

        IImmutableList<IProject> FindProjectsByPackage(string packageId);

        void Clear();
    }
}
