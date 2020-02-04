namespace BeaverSoft.Texo.Core.Console.Rendering
{
    public interface IViewChangesManager
    {
        void AddChange(int index);

        void AddChange(int start, int length);

        void Reset();
    }
}
