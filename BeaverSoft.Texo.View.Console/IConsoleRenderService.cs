using BeaverSoft.Texo.Core.Model.View;

namespace BeaverSoft.Texo.View.Console
{
    public interface IConsoleRenderService
    {
        void Write(IItem item);
    }
}