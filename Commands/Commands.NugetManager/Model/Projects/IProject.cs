using System;
using System.Collections.Immutable;
using BeaverSoft.Texo.Commands.NugetManager.Packages;

namespace BeaverSoft.Texo.Commands.NugetManager.Projects
{
    public interface IProject
    {
        string Id { get; }

        Uri Path { get; }

        IImmutableList<IPackage> Packages { get; }
    }
}
