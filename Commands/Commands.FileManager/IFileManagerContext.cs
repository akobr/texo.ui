using System;
using System.Collections.Generic;
using System.Text;

namespace BeaverSoft.Texo.Commands.FileManager
{
    public interface IFileManagerContext
    {
        ImmutableList<string> Paths
    }
}
