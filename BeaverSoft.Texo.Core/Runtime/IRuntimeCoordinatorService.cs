using System;

namespace BeaverSoft.Texo.Core.Runtime
{
    public interface IRuntimeCoordinatorService : IDisposable
    {
        void Process(string input);
    }
}