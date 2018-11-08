using System;
using System.Collections.Immutable;
using BeaverSoft.Texo.Commands.NugetManager.Model.Packages;

namespace BeaverSoft.Texo.Commands.NugetManager.Model.Projects
{
    public interface IProject
    {
        string Id { get; }

        Uri Path { get; }

        IImmutableDictionary<string, IPackage> Packages { get; }
    }
}
