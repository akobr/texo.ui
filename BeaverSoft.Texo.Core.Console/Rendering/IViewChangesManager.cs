using System.Collections.Generic;

namespace BeaverSoft.Texo.Core.Console.Rendering
{
    public interface IViewChangesManager
    {
        void AddChange(int index);

        void AddChange(int start, int length);

        void Start(int screenStart, int screenLenght, int lineWidth, int cursor);

        IReadOnlyCollection<Sequence> Finish(int screenStart, int screenLenght, int lineWidth, int cursor);

        void Reset();
    }
}
