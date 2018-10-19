using System;
using System.Collections.Immutable;
using BeaverSoft.Texo.Core.Input;
using BeaverSoft.Texo.Core.Model.View;
using BeaverSoft.Texo.Core.View;

namespace BeaverSoft.Texo.View.WPF
{
    public class WpfViewService : IViewService
    {
        public void Render(IInput input)
        {
            throw new NotImplementedException();
        }

        public void Render(IImmutableList<IItem> items)
        {
            throw new NotImplementedException();
        }

        public void RenderIntellisence(IImmutableList<IItem> items)
        {
            throw new NotImplementedException();
        }

        public void RenderProgress(IProgress progress)
        {
            throw new NotImplementedException();
        }

        public void UpdateCurrentDirectory(string directoryPath)
        {
            throw new NotImplementedException();
        }

        public void Update(string key, IItem item)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }

        public void Initialise()
        {
            throw new NotImplementedException();
        }
    }
}
