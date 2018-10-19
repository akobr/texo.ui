using System;
using StrongBeaver.Core;

namespace BeaverSoft.Texo.Core.Runtime
{
    public interface IRuntimeCoordinatorService : IInitialisable, IDisposable
    {
        void Process(string input);
    }
}