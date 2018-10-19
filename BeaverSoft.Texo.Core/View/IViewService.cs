using System;
using System.Collections.Immutable;
using BeaverSoft.Texo.Core.Input;
using BeaverSoft.Texo.Core.Model.View;
using StrongBeaver.Core;

namespace BeaverSoft.Texo.Core.View
{
    public interface IViewService : IInitialisable, IDisposable
    {
        void Render(IInput input);

        void Render(IImmutableList<IItem> items);

        void RenderIntellisence(IImmutableList<IItem> items);

        void RenderProgress(IProgress progress);

        void UpdateCurrentDirectory(string directoryPath);

        void Update(string key, IItem item);
    }
}
