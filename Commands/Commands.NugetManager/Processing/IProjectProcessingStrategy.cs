using System;
using BeaverSoft.Texo.Commands.NugetManager.Projects;

namespace BeaverSoft.Texo.Commands.NugetManager.Processing
{
    public interface IProjectProcessingStrategy
    {
        IProject Process(string filePath);
    }
}
