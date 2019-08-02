using BeaverSoft.Texo.Core.Commands;
using System;
using System.Collections.Generic;

namespace BeaverSoft.Texo.Core.Extensibility
{
    // TODO: [P2] Implement plug-in system of commands
    public class BuiltCommand : ModularCommand, IDisposable
    {
        private readonly HashSet<IDisposable> disposableContainer;

        public BuiltCommand()
        {
            disposableContainer = new HashSet<IDisposable>();
        }

        public void RegisterDisposable(IDisposable disposable)
        {
            disposableContainer.Add(disposable);
        }

        public void Dispose()
        {
            foreach (IDisposable disposable in disposableContainer)
            {
                disposable.Dispose();
            }

            disposableContainer.Clear();
        }
    }
}
