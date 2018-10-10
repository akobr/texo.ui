using System.Collections.Immutable;
using BeaverSoft.Texo.Core.Input;
using BeaverSoft.Texo.Core.Model.View;

namespace BeaverSoft.Texo.Core.View
{
    public interface IViewService
    {
        void Render(IInput input);

        void Render(IImmutableList<IItem> items);

        void RenderIntellisence(IImmutableList<IItem> items);

        void RenderProgress();

        void Update(string key, IItem item);
    }
}
