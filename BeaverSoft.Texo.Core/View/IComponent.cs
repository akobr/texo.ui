using BeaverSoft.Texo.Core.Model.View;

namespace BeaverSoft.Texo.Core.View
{
    public interface IComponent
    {

    }

    public interface IComponent<in T> : IComponent
        where T : IItem
    {
        void Render(T item);
    }
}