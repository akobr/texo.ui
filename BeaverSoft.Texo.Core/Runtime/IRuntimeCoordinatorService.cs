using System;
using StrongBeaver.Core;

namespace BeaverSoft.Texo.Core.Runtime
{
    public interface IRuntimeCoordinatorService : IExecutor, IInitialisable, IDisposable
    {
        void Start();
    }
}